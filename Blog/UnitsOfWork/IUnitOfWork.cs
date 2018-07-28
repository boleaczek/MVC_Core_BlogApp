using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.UnitsOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
        int Save();
    }
}
