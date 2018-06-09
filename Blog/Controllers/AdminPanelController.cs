﻿using Blog.Models;
using Blog.Models.Other;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class AdminPanelController : Controller
    {
        private readonly BlogContext _context;
        BlogData _blogData;
        IHostingEnvironment _hostingEnvironment;
        public AdminPanelController(BlogContext context, BlogData blogData, IHostingEnvironment env)
        {
            _context = context;
            _blogData = blogData;
            _hostingEnvironment = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ICollection<Post> posts = await _context.Posts.Include(t => t.PostTags).
                ThenInclude(t => t.Tag).
                OrderBy(post => post.PublicationDate).ToListAsync();

            return View(new PostsBlogDataViewModel() { BlogData = _blogData, Posts = posts });
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
        public async Task<IActionResult> ModifyBlogData(BlogData blogData)
        {
            _blogData.SaveData(blogData);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Post post = _context.Posts.Include(p => p.Comments).Include(p => p.PostTags).Single(p => p.Id == id);
            _context.RemoveRange(post.Comments);
            _context.Remove(post);
            await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Modify(int id)
        {
            Post post = await _context.Posts.Where(p => p.Id == id)
                .Include(p => p.PostTags)
                .ThenInclude(e => e.Tag)
                .FirstOrDefaultAsync();

            PostTagNameViewModel postTagNameViewModel = new PostTagNameViewModel() { Post = post, TagNames = PostTagsToStringHelper(post.PostTags) };
            return View(postTagNameViewModel);
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
            post.PublicationDate = DateTime.Now;
            _context.Update(post);
            AddTags(post, postTagNameViewModel.TagNames);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Modify(PostTagNameViewModel postTagNameViewModel)
        {
            Post post = await _context.Posts.Where(p => p.Id == postTagNameViewModel.Post.Id)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .SingleOrDefaultAsync();
            
            _context.RemoveRange(post.PostTags);
            await _context.SaveChangesAsync();
            _context.Update(post);
            AddTags(post, postTagNameViewModel.TagNames);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        void AddTags(Post post, string tags_string)
        {
            string[] tag_names = tags_string.Split();
            Tag[] tags = new Tag[tag_names.Length];

            for (int i = 0; i < tag_names.Length; i++)
            {
                tags[i] = _context.Tags.SingleOrDefault(name => name.Name == tag_names[i]);
                if (tags[i] == null)
                {
                    tags[i] = new Tag() { Name = tag_names[i] };
                    _context.Update(tags[i]);
                }
                _context.Add(new PostTag() { Tag = tags[i], Post = post });
            }
        }

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

        void RemovePostTags(ICollection<PostTag> postTags, ICollection<Tag> tags)
        {
        }

        string PostTagsToStringHelper(ICollection<PostTag> tags)
        {
            string tags_string = "";
            foreach (var posttag in tags)
            {
                tags_string += posttag.Tag.Name + " ";
            }

            return tags_string;
        }
    }
}
