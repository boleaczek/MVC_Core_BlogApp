using Blog.Models;
using Blog.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.UnitsOfWork
{
    public interface IBlogUnitOfWork
    {
        IRepository<Post> Posts { get; set; }
        IRepository<Tag> Tags { get; set; }
        IRepository<Comment> Comments { get; set; }
        IRepository<PostTag> PostTags { get; set; }

        Task<int> SaveAsync();
        int Save();
    }
}
