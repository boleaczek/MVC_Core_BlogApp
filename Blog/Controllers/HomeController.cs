using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Blog.Models.Other;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Controllers
{
    public class HomeController : BlogController
    {
        // GET: /<controller>/
        private readonly BlogContext _context;
        BlogData _blogData;

        public HomeController(BlogContext context, BlogData blogData):base(blogData)
        {
            _context = context;
            _blogData = blogData;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Index";
            ViewBag.PagesCount = _context.Posts.Count() / 5;
            return View(await GetPostTagDataViewModel(0));
        }

        public async Task<IActionResult> Pages(int page)
        {
            ViewData["Title"] = "Pages";
            ViewBag.PagesCount = _context.Posts.Count() / 5;
            return View(await GetPostTagDataViewModel(page));
        }

        async Task<PostsTagsViewModel> GetPostTagDataViewModel(int page)
        {
            ICollection<Post> posts = await FindPosts(page);
            ICollection<Tag> tags = await _context.Tags.ToListAsync();
            return new PostsTagsViewModel() { Tags = tags, Posts = posts};
        }

        async Task<ICollection<Post>> FindPosts(int page)
        {
            ICollection<Post> posts = await _context.Posts.Include(t => t.PostTags)
                .ThenInclude(t => t.Tag)
                .OrderBy(post => post.PublicationDate)
                .Skip(5 * page)
                .Take(5).ToListAsync();
            return posts;
        }

        [HttpGet]
        public async Task<IActionResult> Read(int id)
        {
            PostCommentViewModel postCommentViewModel = new PostCommentViewModel()
            {
                Post = await _context.Posts.Include(p => p.Comments).SingleOrDefaultAsync(p => p.Id == id)
            };
            ViewData["Title"] = postCommentViewModel.Post.Title;
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
