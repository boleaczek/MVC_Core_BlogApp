using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class UserManagerViewModel
    {
        public AccountViewModel accountViewModel { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}
