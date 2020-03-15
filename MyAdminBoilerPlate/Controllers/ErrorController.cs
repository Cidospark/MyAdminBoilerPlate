using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAdminBoilerPlate.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /<controller>/
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandeler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry resource not found";
                    break;

                case 500:
                    ViewBag.ErrorMessage = "Sorry resource not found";
                    break;
            }
            return View("NotFound");
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error(int statusCode)
        {
            var exceptionHandlerPathFeatures = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ExceptionPath = exceptionHandlerPathFeatures.Path;
            ViewBag.ExceptionMessage = exceptionHandlerPathFeatures.Error.Message;
            ViewBag.ExceptionStackTrace = exceptionHandlerPathFeatures.Error.StackTrace;
            
            return View("Error");
        }

    }
}

// this page doesn't work