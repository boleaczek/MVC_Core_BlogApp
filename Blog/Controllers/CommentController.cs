using Blog.Models;
using Blog.Models.ViewModels;
using Blog.UnitsOfWork;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        IBlogUnitOfWork _blogUnitOfWork;

        public CommentsController(IBlogUnitOfWork blogUnitOfWork)
        {
            _blogUnitOfWork = blogUnitOfWork;
        }

        [HttpGet]
        [EnableCors("AjaxCommentsPolicy")]
        public async Task<string> Get(int id,
            [FromQuery(Name = "loaded")] int howManyLoaded)
        {
            ICollection<Comment> comments = await _blogUnitOfWork.Comments
                .SearchFor(c => c.Post.Id == id)
                .ToListAsync();

            CommentViewModel commentViewModel = new CommentViewModel()
            {
                Comments = comments.Skip(howManyLoaded).Take(5).ToList(),
                IsLast = (howManyLoaded + comments.Count) >= comments.Count
            };

            Thread.Sleep(10000);

            return JsonConvert.SerializeObject(commentViewModel);
        }
    }
}
