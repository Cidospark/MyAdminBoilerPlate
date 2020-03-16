using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IUserRepository userRepository, IHostingEnvironment hostingEnvironment)
        {
            this.userRepository = userRepository;
            this.hostingEnvironment = hostingEnvironment;
        }


        public string Index()
        {
            return userRepository.GetUser(1).LastName + " " + userRepository.GetUser(1).FirstName;
        }


        public IActionResult Details(int? id)
        {
            // un-comment the line beloww to test the nlog functionality
            //throw new Exception("Error in Details");
            User user = userRepository.GetUser(id??1);

            if(user == null)
            {
                Response.StatusCode = 404;
                return View("IdNotFound");
            }

            HomeDetailsViewModel hdvm = new HomeDetailsViewModel()
            {
                pageTitle = "Users Details",
                user = user
            };

            //var model = userRepository.GetUser(1);
            //ViewData["Title"] = "Users Details";
            return View(hdvm);
        }


        public IActionResult ListOfUsers()
        {
            var users = userRepository.GetAllUsers();
            TempData["numOfUsers"] = userRepository.GetAllUsers().Count();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateUserViewModel userModel)
        {
            // check if all required fields are filled
            if (ModelState.IsValid)
            {
                string uniqueFilename = ProcessUploadedFile(userModel);

                // create new instance of user with the details from the userViewModel instance
                User newUser = new User
                {
                    LastName = userModel.LastName,
                    FirstName = userModel.FirstName,
                    Gender = userModel.Gender,
                    Photo = uniqueFilename
                };

                userRepository.AddUser(newUser);
                return RedirectToAction("ListOfUsers");
            }
            return View();
        }

        private string ProcessUploadedFile(CreateUserViewModel userModel)
        {
            string uniqueFilename = null;

            // check if a photo is selected
            if (userModel.Pix != null)
            {
                // combine the absolute physical path of wwwroot folder with the images folder
                string uploadsFld = Path.Combine(hostingEnvironment.WebRootPath, "images");

                // Combine filename with unique id to implement cache busting
                uniqueFilename = Guid.NewGuid().ToString() + "_" + userModel.Pix.FileName;

                // Combine uploads folder path and filename to get the file path
                string filePath = Path.Combine(uploadsFld, uniqueFilename);

                // Copy file path to the server
                using(var filestream = new FileStream(filePath, FileMode.Create))
                {
                    userModel.Pix.CopyTo(filestream);
                }
                
            }

            return uniqueFilename;
        }

        public IActionResult Delete(int Id)
        {
            var user = userRepository.GetUser(Id);

            if(userRepository.DeleteUser(Id) == null)
            {
                TempData["message"] = "Delete operation failed!"; 
                return RedirectToAction("ListOfUsers");
            }
                
                if(user.Photo != null)
                {
                    string fPath = Path.Combine(hostingEnvironment.WebRootPath, "images", user.Photo);
                    System.IO.File.Delete(fPath);
                
                }

            TempData["message"] = "Deleted successfully!";
            return RedirectToAction("ListOfUsers");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = userRepository.GetUser(id);

            if (user == null)
            {
                Response.StatusCode = 404;
                return View("IdNotFound");
            }

            EditUserViewModel editedUser = new EditUserViewModel
            {
                UserId = user.UserId,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Gender = user.Gender,
                ExistingPhotoPath = user.Photo
            };
            return View(editedUser);
        }
        [HttpPost]
        public IActionResult Edit(EditUserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                // get the particular user to edit
                var newUser = userRepository.GetUser(userModel.UserId);
                newUser.LastName = userModel.LastName;
                newUser.FirstName = userModel.FirstName;
                newUser.Gender = userModel.Gender;
                
                // check if a new picture upload is selected
                if(userModel.Pix != null)
                {
                    // check if this user already have an existing photo
                    if(userModel.ExistingPhotoPath != null)
                    {
                        // combine paths to get unique filepath
                        string filepath = Path.Combine(hostingEnvironment.WebRootPath, "images", userModel.ExistingPhotoPath);

                        // delete existing file path
                        System.IO.File.Delete(filepath);
                    }
                    newUser.Photo = ProcessUploadedFile(userModel);
                }

                if (userRepository.EditUser(newUser) == null)
                {
                    TempData["message"] = "Update operation failed!";
                    return RedirectToAction("ListOfUsers");
                }
                TempData["message"] = "Updated successfully!";
            }
            return RedirectToAction("ListOfUsers");
        }
    }
}
