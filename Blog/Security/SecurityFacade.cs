using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Security
{
    public class SecurityFacade : ISecurityFacade
    {
        readonly IHttpContextAccessor _httpContextAccesor;
        readonly IAuthorizationService _authorizationService;
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly UserManager<ApplicationUser> _userManager;

        public SecurityFacade(IHttpContextAccessor httpContextAccesor, IAuthorizationService authorizationService,
            SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccesor = httpContextAccesor;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> IsAuthorized(object resource, string requirementName)
        {
            OperationAuthorizationRequirement authorizationRequirement = new OperationAuthorizationRequirement() { Name = requirementName };
            var authorized = await _authorizationService.AuthorizeAsync(_httpContextAccesor.HttpContext.User, resource, authorizationRequirement);
            return authorized.Succeeded;
        }

        public bool IsInRole(string roleName)
        {
            return _httpContextAccesor.HttpContext.User.IsInRole(roleName);
        }

        public async Task<bool> LogIn(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            return result.Succeeded;
        }
    }
}
