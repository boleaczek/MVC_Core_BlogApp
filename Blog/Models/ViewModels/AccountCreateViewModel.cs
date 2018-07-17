using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.ViewModels
{
    public class AccountCreateViewModel
    {
        public AccountLoginViewModel LoginData { get; set; }
        public bool IsAdmin { get; set; }
    }
}
