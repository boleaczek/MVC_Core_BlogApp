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

namespace Blog.Controllers
{
    [Authorize]
    public class AdminPanelController : Controller
    {
        private readonly IBlogUnitOfWork _blogUnitOfWork;
        BlogData _blogData;
        UserContext _userContext;
        UserManager<ApplicationUser> _userManager;
        IHostingEnvironment _hostingEnvironment;

        public AdminPanelController(UserManager<ApplicationUser> userManager, BlogData blogData, UserContext userContext, IHostingEnvironment env, IBlogUnitOfWork blogUnitOfWork)
        {
            _blogData = blogData;
            _hostingEnvironment = env;
            _blogUnitOfWork = blogUnitOfWork;
            _userContext = userContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ICollection<Post> posts = await _blogUnitOfWork.Posts
                .GetAll()
                .OrderBy(post => post.PublicationDate)
                .ToListAsync();
            ICollection<Tag> tags = await _blogUnitOfWork.Tags.GetAll().ToListAsync();

            return View(new AdminPanelViewModel() { BlogData = _blogData, Posts = posts, Tags = tags });
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
        public async Task<IActionResult> Modify(int id)
        {
            Post post = await _blogUnitOfWork.Posts.SearchFor(p => p.Id == id)
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
            _blogUnitOfWork.Posts.Insert(post);
            AddTags(post, postTagNameViewModel.TagNames);

            await _blogUnitOfWork.SaveAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Modify(PostTagNameViewModel postTagNameViewModel)
        {
            Post post = await _blogUnitOfWork.Posts.SearchFor(p => p.Id == postTagNameViewModel.Post.Id)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .SingleOrDefaultAsync();

            _blogUnitOfWork.PostTags.DeleteMany(post.PostTags);
            await _blogUnitOfWork.SaveAsync();

            post.Title = postTagNameViewModel.Post.Title;
            post.Content = postTagNameViewModel.Post.Content;

            _blogUnitOfWork.Posts.Update(post);
            AddTags(post, postTagNameViewModel.TagNames);
            await _blogUnitOfWork.SaveAsync();

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

        [HttpGet]
        public async Task<IActionResult> UserManager()
        {
            ICollection<ApplicationUser> users = await _userContext.Users.ToListAsync();
            return View(new UserManagerViewModel() { Users = users });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AccountViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            return RedirectToAction("UserManager");
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
