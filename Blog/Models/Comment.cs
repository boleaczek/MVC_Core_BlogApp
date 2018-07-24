using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Comment : IBlogResource
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime PublicationDate { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
    }
}
