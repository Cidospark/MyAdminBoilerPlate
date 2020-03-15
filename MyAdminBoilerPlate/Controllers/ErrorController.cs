using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAdminBoilerPlate.Controllers
{
    public class ErrorController : Controller
    {
        // create a constructor to inject the ILogger interface with a generic type
        // of the current class for easy tracing of the error path namespace
        
        private readonly ILogger _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        // GET: /<controller>/
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandeler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry resource not found";
                    _logger.LogWarning($"404 Error Occurred. Path={statusCodeResult.OriginalPath}" +
                        $"and QueryString = {statusCodeResult.OriginalQueryString}");
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

            //ViewBag.ExceptionPath = exceptionHandlerPathFeatures.Path;
            //ViewBag.ExceptionMessage = exceptionHandlerPathFeatures.Error.Message;
            //ViewBag.ExceptionStackTrace = exceptionHandlerPathFeatures.Error.StackTrace;

            _logger.LogError($"The path {exceptionHandlerPathFeatures.Path} " +
                $"threw the exception {exceptionHandlerPathFeatures.Error}");

            return View("Error");
        }

    }
}

// this page doesn't work