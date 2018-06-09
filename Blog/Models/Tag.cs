using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<PostTag> PostTags { get; } = new List<PostTag>();
    }
}
