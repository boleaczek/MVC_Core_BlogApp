using Blog.Models.Other;
using Blog.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class ErrorController : BlogController
    {
        public ErrorController(BlogData data) : base(data)
        {
        }

        [HttpGet]
        public void test()
        {
            throw new ApplicationException();
        }

        [Route("/Error")]
        [HttpGet]
        public IActionResult UnhandledException()
        {
            return View(new ErrorViewModel() { Message = "Error has occured." });
        }

        [Route("/Error/404")]
        [HttpGet]
        public IActionResult Error404()
        {
            return View();
        }
    }
}
