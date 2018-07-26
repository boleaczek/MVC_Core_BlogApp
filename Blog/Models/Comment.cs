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
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }
        [Required]
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }
        [JsonProperty(PropertyName = "publicationDate")]
        public DateTime PublicationDate { get; set; }
        [JsonIgnore]
        public Post Post { get; set; }
    }
}
