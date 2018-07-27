using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Security
{
    public interface ISecurityFacade
    {
        Task<bool> LogIn(string username, string password);
        Task<bool> IsAuthorized(object resource, string requirementName);
        bool IsInRole(string roleName);
    }
}
