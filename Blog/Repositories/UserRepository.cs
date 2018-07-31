using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories
{
    public class UserRepository : IUserRepository
    {
        UserManager<ApplicationUser> _userManager;
        DbSet<ApplicationUser> _users;

        public UserRepository(UserManager<ApplicationUser> userManager, UserContext context)
        {
            _userManager = userManager;
            _users = context.Set<ApplicationUser>();
        }

        public async Task Delete(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }

        public async Task DeleteMany(ICollection<ApplicationUser> users)
        {
            ICollection<Task> tasks = new List<Task>();
            foreach (var user in users)
            {
                tasks.Add(_userManager.DeleteAsync(user));
            }

            await Task.WhenAll(tasks);
        }

        public IQueryable<ApplicationUser> GetAll()
        {
            return _users;
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            return await _users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task Insert(ApplicationUser user, string password)
        {
            await _userManager.CreateAsync(user, password);
        }

        public IQueryable<ApplicationUser> SearchFor(Expression<Func<ApplicationUser, bool>> predicate)
        {
            return _users.Where(predicate);
        }

        public async Task Update(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }
    }
}
