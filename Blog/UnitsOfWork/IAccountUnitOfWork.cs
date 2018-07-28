using Blog.Models;
using Blog.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.UnitsOfWork
{
    public interface IAccountUnitOfWork : IUnitOfWork
    {
        IUserRepository Users { get; set; }
        IRepository<IdentityRole> Roles { get; set;}
        IRepository<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
