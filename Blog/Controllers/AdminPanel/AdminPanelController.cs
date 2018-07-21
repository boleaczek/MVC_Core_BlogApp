using Blog.Security;
using Blog.Models;
using Blog.Models.Other;
using Blog.Models.ViewModels;
using Blog.UnitsOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers.AdminPanel
{
    [Authorize]
    public class AdminPanelController : Controller
    {
        private readonly IBlogUnitOfWork _blogUnitOfWork;
        BlogData _blogData;
        IHostingEnvironment _hostingEnvironment;
        private readonly IAuthorizationService _authorizationService;

        public AdminPanelController(BlogData blogData, IHostingEnvironment env, IBlogUnitOfWork blogUnitOfWork, IAuthorizationService authorizationService)
        {
            _blogData = blogData;
            _hostingEnvironment = env;
            _blogUnitOfWork = blogUnitOfWork;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole(BlogConstants.AdministratorRoleName))
            {
                return RedirectToAction("Index", "WriterPanel");
            }

            ICollection<Post> posts = await _blogUnitOfWork.Posts
                .GetAll()
                .OrderBy(post => post.PublicationDate)
                .ToListAsync();
            ICollection<Tag> tags = await _blogUnitOfWork.Tags.GetAll().ToListAsync();

            return View(new AdminPanelViewModel() { BlogData = _blogData, Posts = posts, Tags = tags, CurrentUser = User });
        }

        [HttpPost]
        public async Task<IActionResult> ModifyBlogData(BlogData blogData)
        {
            var authorized = await _authorizationService.AuthorizeAsync(User, _blogData, BlogAuthorization.Modify);

            if (authorized.Succeeded)
            {
                _blogData.SaveData(blogData);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeletePost(int id)
        {
            Post post = _blogUnitOfWork.Posts.GetAll().Include(p => p.Comments).Include(p => p.PostTags).Single(p => p.Id == id);

            var authorized = await _authorizationService.AuthorizeAsync(User, post, BlogAuthorization.Delete);
            if (authorized.Succeeded)
            {
                _blogUnitOfWork.Comments.DeleteMany(post.Comments);
                _blogUnitOfWork.Posts.Delete(post);
                await _blogUnitOfWork.SaveAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadLogo(IFormFile logo)
        {
            var authorized = await _authorizationService.AuthorizeAsync(User, _blogData, BlogAuthorization.Modify);

            if (authorized.Succeeded)
            {
                await UploadFile("logo.png", logo);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadIcon(IFormFile icon)
        {
            var authorized = await _authorizationService.AuthorizeAsync(User, _blogData, BlogAuthorization.Modify);

            if (authorized.Succeeded)
            {
                await UploadFile("icon.ico", icon);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CommentManager(int id)
        {
            ICollection<Comment> comments = await _blogUnitOfWork.Comments
                .SearchFor(c => c.Post.Id == id).ToListAsync();
            return View(comments);
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            Comment comment = await _blogUnitOfWork.Comments.GetAll().Include(c => c.Post).SingleOrDefaultAsync(c => c.Id == id);

            var authorized = await _authorizationService.AuthorizeAsync(User, comment, BlogAuthorization.Delete);
            int redir_id = comment.Post.Id;

            if (authorized.Succeeded)
            {
                _blogUnitOfWork.Comments.Delete(comment);
                await _blogUnitOfWork.SaveAsync();
            }

            return RedirectToAction("CommentManager", new { id = redir_id });
        }

        public async Task<IActionResult> DeleteTag(int id)
        {
            Tag tag = await _blogUnitOfWork.Tags.GetById(id);

            var authorized = await _authorizationService.AuthorizeAsync(User, tag, BlogAuthorization.Delete);

            if (authorized.Succeeded)
            {
                _blogUnitOfWork.Tags.Delete(tag);
                await _blogUnitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        #region helpers

        [NonAction]
        ICollection<Tag> GetTags(string tags_string)
        {
            string[] tag_names = tags_string.Trim().Split();
            ICollection<Tag> tags = new Tag[tag_names.Length];

            foreach (string tag_name in tag_names)
            {
                tags.Add(new Tag() { Name = tag_name });
            }

            return tags;
        }

        [NonAction]
        async Task UploadFile(string path, IFormFile file)
        {
            var filePath = _hostingEnvironment.WebRootPath;
            filePath += $@"\{path}";

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        #endregion
    }
}
