using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public ICollection<Comment> Comments { get; } = new List<Comment>();
        public ICollection<PostTag> PostTags { get; } = new List<PostTag>();
    }
}
