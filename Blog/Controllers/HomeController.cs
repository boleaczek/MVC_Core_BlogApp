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
        public async Task<IActionResult> Read(int id)
        {
            PostCommentViewModel postCommentViewModel = new PostCommentViewModel()
            {
                Post = await _context.Posts.Include(p => p.Comments).SingleOrDefaultAsync(p => p.Id == id)
            };
            return View(postCommentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> WriteComment(PostCommentViewModel postCommentViewModel)
        {
            int id = postCommentViewModel.Post.Id;
            Comment comment = postCommentViewModel.Comment;

            Post post = await _context.Posts.Include(p => p.Comments).SingleOrDefaultAsync(p => p.Id == id);
            
            if(post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            _context.Comments.Update(comment);
            post.Comments.Add(comment);
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Read/" + post.Id.ToString());
        }
    }
}
