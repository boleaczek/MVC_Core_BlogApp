using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class ApplicationUser : IdentityUser, IBlogResource
    {
        public override string Id { get; set; }
        [PersonalData]
        public string Name { get; set; }
    }
}
