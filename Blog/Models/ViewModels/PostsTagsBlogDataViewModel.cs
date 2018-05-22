using Blog.Models.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostsTagsViewModel
    {
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
