using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.UnitsOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        DbContext _context;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }

        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
