using Blog.Models.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class BlogController : Controller
    {
        protected BlogData _blogData;

        public BlogController(BlogData data)
        {
            _blogData = data;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            ViewData.Add("AuthorName", _blogData.AuthorName);
            ViewData.Add("MailAddres", _blogData.MailAddress);
            ViewData.Add("Description", _blogData.Description);
            ViewData.Add("BlogName", _blogData.BlogName);
            base.OnActionExecuted(context);
        }
    }
}
