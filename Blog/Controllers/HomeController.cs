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
using Blog.UnitsOfWork;
using Blog.Models.ViewModels;

namespace Blog.Controllers
{
    public class HomeController : BlogController
    {
        // GET: /<controller>/
        private readonly IBlogUnitOfWork unitOfWork;

        public HomeController(BlogData blogData, IBlogUnitOfWork blogUnitOfWork):base(blogData)
        {
            _blogData = blogData;
            unitOfWork = blogUnitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Index";
            ViewBag.PagesCount = unitOfWork.Posts.GetAll().Count() / 5;
            return View(await GetPostTagDataViewModel(0));
        }

        public async Task<IActionResult> Pages(int page)
        {
            ViewData["Title"] = "Pages";
            ViewBag.PagesCount = unitOfWork.Posts.GetAll().Count() / 5;
            return View(await GetPostTagDataViewModel(page));
        }

        [HttpGet]
        public async Task<IActionResult> Read(int id)
        {
            Post post = await unitOfWork.Posts.GetById(id);
            post.Comments = await unitOfWork.Comments.SearchFor(comment => comment.Post.Id == id).ToListAsync();
            PostCommentViewModel postCommentViewModel = new PostCommentViewModel()
            {
                Post = post
            };
            ViewData["Title"] = post.Title;
            return View(postCommentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> WriteComment(PostCommentViewModel postCommentViewModel)
        {
            int id = postCommentViewModel.Post.Id;
            Comment comment = postCommentViewModel.Comment;

            Post post = await unitOfWork.Posts.GetById(id);
            
            if(post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            unitOfWork.Comments.Insert(comment);
            post.Comments.Add(comment);
            
            await unitOfWork.SaveAsync();

            return RedirectToAction("Read/" + post.Id.ToString());
        }

        #region helpers

        [NonAction]
        async Task<PostsTagsViewModel> GetPostTagDataViewModel(int page)
        {
            ICollection<Post> posts = await FindPosts(page);
            ICollection<Tag> tags = await unitOfWork.Tags.GetAll().ToListAsync();
            return new PostsTagsViewModel() { Tags = tags, Posts = posts };
        }

        [NonAction]
        async Task<ICollection<Post>> FindPosts(int page)
        {
            ICollection<Post> posts = await unitOfWork.Posts.GetAll()
                .OrderBy(post => post.PublicationDate)
                .Skip(5 * page)
                .Take(5)
                .ToListAsync();
            return posts;
        }

        #endregion
    }
}
