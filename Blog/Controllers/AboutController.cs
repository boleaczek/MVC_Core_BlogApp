using Blog.Models.Other;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blog.Controllers
{
    public class AboutController : BlogController
    {
        public AboutController(BlogData blogData) : base(blogData)
        {

        }

        public string GetInfo()
        {
            return JsonConvert.SerializeObject(_blogData);
        }

        public IActionResult Index()
        {
            return View(_blogData);
        }
    }
}
