﻿using Blog.Authorization;
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

        public AdminPanelController(BlogData blogData, IHostingEnvironment env, IBlogUnitOfWork blogUnitOfWork)
        {
            _blogData = blogData;
            _hostingEnvironment = env;
            _blogUnitOfWork = blogUnitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(!User.IsInRole(BlogConstants.AdministratorRoleName))
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
            _blogData.SaveData(blogData);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeletePost(int id)
        {
            Post post = _blogUnitOfWork.Posts.GetAll().Include(p => p.Comments).Include(p => p.PostTags).Single(p => p.Id == id);
            _blogUnitOfWork.Comments.DeleteMany(post.Comments);
            _blogUnitOfWork.Posts.Delete(post);
            await _blogUnitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadLogo(IFormFile logo)
        {
            var filePath = _hostingEnvironment.WebRootPath;
            filePath += @"\logo.png";
            _blogData.ImagePath = @"\logo.png";
            _blogData.SaveData(_blogData);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logo.CopyToAsync(stream);
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
            _blogUnitOfWork.Comments.Delete(comment);
            int redir_id = comment.Post.Id;
            await _blogUnitOfWork.SaveAsync();
            return RedirectToAction("CommentManager", new { id = redir_id });
        }

        public async Task<IActionResult> DeleteTag(int id)
        {
            Tag tag = await _blogUnitOfWork.Tags.GetById(id);
            _blogUnitOfWork.Tags.Delete(tag);
            await _blogUnitOfWork.SaveAsync();
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


        #endregion
    }
}
