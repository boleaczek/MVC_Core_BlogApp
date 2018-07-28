using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Security;
using Blog.Models;
using Blog.Models.ViewModels;
using Blog.UnitsOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers.AdminPanel
{
    [Authorize]
    public class WriterPanelController : Controller
    {
        protected IBlogUnitOfWork _blogUnitOfWork;
        ISecurityFacade _securityFacade;

        public WriterPanelController(IBlogUnitOfWork blogUnitOfWork, ISecurityFacade securityFacade)
        {
            _blogUnitOfWork = blogUnitOfWork;
            _securityFacade = securityFacade;
        }

        public async Task<IActionResult> Index()
        {
            ICollection<Post> posts = await _blogUnitOfWork.Posts.GetAll().ToListAsync();
            string writerId = _securityFacade.GetCurrentUserId();
            
            return View(
                new WritersListViewModel()
                {
                    Posts = posts,
                    WriterId = writerId
                });
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

            ApplicationUser author = await _securityFacade.GetCurrentUser();

            post.PublicationDate = DateTime.Now;
            post.AuthorName = author.Name;
            post.AuthorUserId = author.Id;
            _blogUnitOfWork.Posts.Insert(post);

            AddTags(post, postTagNameViewModel.TagNames);

            await _blogUnitOfWork.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Modify(int id)
        {
            Post post = await _blogUnitOfWork.Posts.SearchFor(p => p.Id == id)
                .Include(p => p.PostTags)
                .ThenInclude(e => e.Tag)
                .FirstOrDefaultAsync();

            PostTagNameViewModel postTagNameViewModel = new PostTagNameViewModel() { Post = post, TagNames = PostTagsToStringHelper(post.PostTags) };

            return View(postTagNameViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Modify(PostTagNameViewModel postTagNameViewModel)
        {
            Post post = await _blogUnitOfWork.Posts.SearchFor(p => p.Id == postTagNameViewModel.Post.Id)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .SingleOrDefaultAsync();

            var authorized = await _securityFacade.IsAuthorized(post, BlogConstants.ModifyActionName);

            if (authorized)
            {
                _blogUnitOfWork.PostTags.DeleteMany(post.PostTags);
                await _blogUnitOfWork.SaveAsync();

                post.Title = postTagNameViewModel.Post.Title;
                post.Content = postTagNameViewModel.Post.Content;

                _blogUnitOfWork.Posts.Update(post);
                AddTags(post, postTagNameViewModel.TagNames);
                await _blogUnitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        #region helpers

        [NonAction]
        void AddTags(Post post, string tags_string)
        {
            string[] tag_names = tags_string.Split();
            Tag[] tags = new Tag[tag_names.Length];

            for (int i = 0; i < tag_names.Length; i++)
            {
                tags[i] = _blogUnitOfWork.Tags.SearchFor(name => name.Name == tag_names[i]).SingleOrDefault();
                if (tags[i] == null)
                {
                    tags[i] = new Tag() { Name = tag_names[i] };
                    _blogUnitOfWork.Tags.Update(tags[i]);
                }

                _blogUnitOfWork.PostTags.Insert(new PostTag() { Tag = tags[i], Post = post });
            }
        }

        [NonAction]
        string PostTagsToStringHelper(ICollection<PostTag> tags)
        {
            string tags_string = "";
            foreach (var posttag in tags)
            {
                tags_string += posttag.Tag.Name + " ";
            }

            return tags_string;
        }
        #endregion
    }
}