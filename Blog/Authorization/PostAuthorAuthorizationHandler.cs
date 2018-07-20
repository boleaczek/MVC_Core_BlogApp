using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Authorization
{
    public class PostAuthorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {
        protected UserManager<ApplicationUser> _userManager;

        public PostAuthorAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Post resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name == BlogConstants.AddActionName)
            {
                context.Succeed(requirement);
            }

            string userId = _userManager.GetUserId(context.User);

            if (userId != resource.AuthorUserId)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name == BlogConstants.ModifyActionName)
            {
                context.Succeed(requirement);
            }

            if (context.User.IsInRole(BlogConstants.AdministratorRoleName))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}