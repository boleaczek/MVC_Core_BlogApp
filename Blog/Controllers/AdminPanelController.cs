using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly BlogContext _context;

        public AdminPanelController(BlogContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(PostTagNameViewModel postTagNameViewModel)
        {
            Post post = postTagNameViewModel.Post;
            _context.Update(post);
            string[] tagNames = postTagNameViewModel.TagNames.Split(' ');
            Tag[] tags = new Tag[tagNames.Length];

            for(int i = 0; i< tagNames.Length; i++)
            {
                tags[i] = _context.Tags.SingleOrDefault(name => name.Name == tagNames[i]);
                if (tags[i] == null)
                {
                    tags[i] = new Tag() { Name = tagNames[i] };
                    _context.Update(tags[i]);
                }
                _context.Update(new PostTag() { Tag = tags[i], Post = post });
            }

            await _context.SaveChangesAsync();
            
            return RedirectToAction("Add");
        }
    }
}
