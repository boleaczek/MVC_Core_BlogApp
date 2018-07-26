using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class CommentViewModel
    {
        [JsonProperty("comments")]
        public ICollection<Comment> Comments { get; set; }
        [JsonProperty("isLast")]
        public bool IsLast { get; set; }
    }
}
