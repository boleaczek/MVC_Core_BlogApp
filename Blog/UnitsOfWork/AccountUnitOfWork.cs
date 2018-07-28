using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Blog.UnitsOfWork
{
    public class AccountUnitOfWork : UnitOfWork, IAccountUnitOfWork
    {
        public IUserRepository Users { get; set; }
        public IRepository<IdentityRole> Roles { get; set; }
        public IRepository<IdentityUserRole<string>> UserRoles { get; set; }

        public AccountUnitOfWork(UserContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            Users = new UserRepository(userManager, context);
            Roles = new Repository<IdentityRole>(context);
            UserRoles = new Repository<IdentityUserRole<string>>(context);
        }
    }
}
