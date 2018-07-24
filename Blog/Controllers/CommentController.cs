using Blog.Models;
using Blog.UnitsOfWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<string> Get(int id,
            [FromQuery(Name = "loaded")] int howManyLoaded)
        {
            ICollection<Comment> comments = await _blogUnitOfWork.Comments
                .SearchFor(c => c.Post.Id == id)
                .Skip(howManyLoaded)
                .Take(5)
                .ToListAsync();

            return JsonConvert.SerializeObject(comments).ToLower();
        }
    }
}
