using Blog.Models;
using Blog.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.UnitsOfWork
{
    public class BlogUnitOfWork : UnitOfWork, IBlogUnitOfWork
    {
        public IRepository<Post> Posts { get;  set; }
        public IRepository<Tag> Tags { get; set; }
        public IRepository<Comment> Comments { get; set; }
        public IRepository<PostTag> PostTags { get; set; }

        public BlogUnitOfWork(BlogContext context) : base(context)
        {
            Posts = new Repository<Post>(context);
            Tags = new Repository<Tag>(context);
            Comments = new Repository<Comment>(context);
            PostTags = new Repository<PostTag>(context);
        }
    }
}
