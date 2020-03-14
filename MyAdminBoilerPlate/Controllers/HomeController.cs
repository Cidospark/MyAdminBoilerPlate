using Microsoft.AspNetCore.Mvc;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;

        public HomeController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }


        public string Index()
        {
            return userRepository.GetUser(1).LastName + " " + userRepository.GetUser(1).FirstName;
        }


        public IActionResult Details(int? id)
        {
            HomeDetailsViewModel hdvm = new HomeDetailsViewModel()
            {
                pageTitle = "Users Details",
                user = userRepository.GetUser(id??1)
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
        public IActionResult Create(User userModel)
        {
            if (ModelState.IsValid)
            {
                userRepository.AddUser(userModel);
                return RedirectToAction("ListOfUsers");
            }
            return View();
        }

        public IActionResult Delete(int Id)
        {
            if(userRepository.DeleteUser(Id) == 0)
            {
                TempData["message"] = "Delete operation failed!"; 
                return RedirectToAction("ListOfUsers");
            }
            TempData["message"] = "Deleted successfully!";
            return RedirectToAction("ListOfUsers");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = userRepository.GetUser(id);
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(User userModel)
        {
            if (ModelState.IsValid)
            {
                if (userRepository.EditUser(userModel) == 0)
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
