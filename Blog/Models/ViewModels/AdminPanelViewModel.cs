using Blog.Models.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class AdminPanelViewModel
    {
        public ICollection<Post> Posts { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public BlogData BlogData { get; set; }
        public ClaimsPrincipal CurrentUser { get; set; }
    }
}
