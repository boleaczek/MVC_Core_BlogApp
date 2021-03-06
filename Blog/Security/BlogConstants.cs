﻿using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Security
{
    public static class BlogAuthorization
    {
        public static readonly OperationAuthorizationRequirement Delete = new  OperationAuthorizationRequirement() { Name = BlogConstants.DeleteActionName };
        public static readonly OperationAuthorizationRequirement Modify = new OperationAuthorizationRequirement() { Name = BlogConstants.ModifyActionName };
        public static readonly OperationAuthorizationRequirement Add = new OperationAuthorizationRequirement() { Name = BlogConstants.AddActionName };
    }

    public static class BlogConstants
    {
        public const string DeleteActionName = "Delete";
        public const string ModifyActionName = "Modify";
        public const string AddActionName = "Add";

        public const string AdministratorRoleName = "Admin";
        public const string WriterRoleName = "Writer";
    }
}
