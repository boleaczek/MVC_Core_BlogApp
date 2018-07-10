using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Blog.Models.Other;
using Blog.UnitsOfWork;

namespace Blog.Controllers
{
    public class ArchiveController : BlogController
    {
        private readonly IBlogUnitOfWork _blogUnitOfWork;

        public ArchiveController(BlogData blogData, IBlogUnitOfWork blogUnitOfWork) : base(blogData)
        {
            _blogUnitOfWork = blogUnitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Archive";
            ICollection<Tag> tags = await _blogUnitOfWork.Tags
                .GetAll()
                .Include(t => t.PostTags)
                .ThenInclude(pt => pt.Post)
                .ToListAsync();
            return View(tags);
        }

        [HttpGet]
        public async Task<bool> IsUsed(string tagName)
        {
            Tag tag = await _blogUnitOfWork.Tags.SearchFor(t => t.Name == tagName).SingleOrDefaultAsync();
            if (tag == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
