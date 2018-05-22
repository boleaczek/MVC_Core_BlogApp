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

namespace Blog.Controllers
{
    public class ArchiveController : BlogController
    {
        private readonly BlogContext _context;

        public ArchiveController(BlogContext context, BlogData blogData) : base(blogData)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Archive";
            ICollection<Tag> tags = await _context.Tags
                .Include(t => t.PostTags)
                .ThenInclude(pt => pt.Post)
                .ToListAsync();
            return View(tags);
        }

        [HttpGet]
        public async Task<bool> IsUsed(string tagName)
        {
            Tag tag = await _context.Tags.SingleOrDefaultAsync(t => t.Name == tagName);
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
