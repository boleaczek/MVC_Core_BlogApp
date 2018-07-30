using Blog.Models;
using Blog.Models.Other;
using Blog.Models.ViewModels;
using Blog.UnitsOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : BlogController
    {
        private readonly IBlogUnitOfWork unitOfWork;

        public HomeController(BlogData blogData, IBlogUnitOfWork blogUnitOfWork):base(blogData)
        {
            _blogData = blogData;
            unitOfWork = blogUnitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.PagesCount = unitOfWork.Posts.GetAll().Count() / 5;
            return View(await GetPostTagDataViewModel(0));
        }

        public async Task<IActionResult> Pages(int page)
        {
            ViewBag.PagesCount = unitOfWork.Posts.GetAll().Count() / 5;
            return View(await GetPostTagDataViewModel(page));
        }

        [HttpGet]
        public async Task<IActionResult> Read(int id)
        {
            Post post = await unitOfWork.Posts
                    .SearchFor(p => p.Id == id)
                    .Include(p => p.Comments)
                    .SingleOrDefaultAsync();
            
            PostCommentViewModel postCommentViewModel = new PostCommentViewModel()
            {
                Post = post
            };
            
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

            comment.PublicationDate = DateTime.Now;

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
