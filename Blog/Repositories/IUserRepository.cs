using Blog.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog.Repositories
{
    public interface IUserRepository
    {
        Task Insert(ApplicationUser user);
        Task Delete(ApplicationUser user);
        Task DeleteMany(ICollection<ApplicationUser> users);
        Task Update(ApplicationUser user);
        IQueryable<ApplicationUser> SearchFor(Expression<Func<ApplicationUser, bool>> predicate);
        IQueryable<ApplicationUser> GetAll();
        Task<ApplicationUser> GetById(string id);
    }
}
