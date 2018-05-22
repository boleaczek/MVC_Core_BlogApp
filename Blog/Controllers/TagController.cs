using Blog.Models;
using Blog.Models.Other;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class TagController : BlogController
    {
        BlogContext _context;

        public TagController(BlogData blogData, BlogContext context) : base(blogData)
        {
            _context = context;
        }

        public async Task<bool> IsUsed(string tagName)
        {
            Tag tag = await _context.Tags.SingleOrDefaultAsync(t => t.Name == tagName);
            if(tag == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
