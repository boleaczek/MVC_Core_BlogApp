using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Authorization
{
    public static class BlogAuthorization
    {
        public static readonly OperationAuthorizationRequirement Delete = new  OperationAuthorizationRequirement() { Name = BlogConstants.DeleteAction };
    }

    public static class BlogConstants
    {
        public const string DeleteAction = "Delete";
        public const string AdministratorRole = "Admin";
    }
}
