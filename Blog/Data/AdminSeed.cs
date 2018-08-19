using Blog.Models;
using Blog.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Data
{
    public static class AdminSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<UserContext>();
            InitializeRoles(context, BlogConstants.AdministratorRoleName);
            InitializeRoles(context, BlogConstants.WriterRoleName);
            InitializeAdmin(userManager, context);
            context.SaveChanges();
        }

        static void InitializeAdmin(UserManager<ApplicationUser> userManager, UserContext context)
        {
            if (!userManager.Users.Any())
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    Email = "admin@blog.com",
                    UserName = "admin@blog.com"
                };

                var res = userManager.CreateAsync(applicationUser, "T23V_Nu46").Result;

                if (res.Succeeded)
                {
                    string adminRoleId = context.Roles.Where(role => role.Name == BlogConstants.AdministratorRoleName).SingleOrDefault().Id;

                    context.UserRoles.Add(
                        new IdentityUserRole<string>()
                        {
                            UserId = applicationUser.Id,
                            RoleId = adminRoleId
                        });
                }
            }
        }

        static void InitializeRoles(UserContext context, string roleName)
        {
            if(context.Roles.Where(role => role.Name == roleName).SingleOrDefault() == null)
            {
                IdentityRole role = new IdentityRole(roleName);
                context.Roles.Add(role);
            }
        }
    }
}
