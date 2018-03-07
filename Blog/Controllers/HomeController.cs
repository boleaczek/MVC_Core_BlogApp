using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        private readonly BlogContext _context;

        public HomeController(BlogContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ICollection<Post> posts = _context.Posts.Include(t => t.PostTags).
                ThenInclude(t => t.Tag).
                OrderBy(post => post.PublicationDate).
                Take(5).ToList();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Read(int id)
        {
            Post post = _context.Posts.SingleOrDefault(p => p.Id == id);
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> WriteComment(int id, Comment comment)
        {
            Post post = _context.Posts.Include(p => p.Comments).SingleOrDefault(p => p.Id == id);
            if(post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            _context.Comments.Update(comment);
            post.Comments.Add(comment);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return Read(post.Id);
        }
    }
}
