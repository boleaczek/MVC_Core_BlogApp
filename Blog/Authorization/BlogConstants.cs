using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Authorization
{
    public static class BlogAuthorization
    {
        public static readonly OperationAuthorizationRequirement Delete = new  OperationAuthorizationRequirement() { Name = BlogConstants.DeleteActionName };
        public static readonly OperationAuthorizationRequirement Modify = new OperationAuthorizationRequirement() { Name = BlogConstants.ModifyActionName };
    }

    public static class BlogConstants
    {
        public const string DeleteActionName = "Delete";
        public const string ModifyActionName = "Modify";

        public const string AdministratorRoleName = "Admin";
    }
}
