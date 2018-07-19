using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class WritersListViewModel
    {
        public ICollection<Post> Posts { get; set; }
        public string WriterId { get; set; }
    }
}
