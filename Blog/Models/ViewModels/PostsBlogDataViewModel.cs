using Blog.Models.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostsBlogDataViewModel
    {
        public ICollection<Post> Posts { get; set; }
        public BlogData BlogData { get; set; }
    }
}
