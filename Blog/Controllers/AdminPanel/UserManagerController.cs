using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Authorization;
using Blog.Models;
using Blog.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers.AdminPanel
{
    [Route("/AdminPanel/UserManager")]
    [Authorize]
    public class UserManagerController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        UserContext _userContext;

        public UserManagerController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, UserContext userContext, IAuthorizationService authorizationService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userContext = userContext;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (UserIsAuthorized())
            {
                ICollection<ApplicationUser> users = await _userContext.Users.ToListAsync();
                return View(new UserManagerViewModel() { Users = users });
            }
            return RedirectToReferer();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AccountCreateViewModel model)
        {
            if (!UserIsAuthorized())
            {
                return RedirectToReferer();
            }

            var user = new ApplicationUser { UserName = model.LoginData.Email, Email = model.LoginData.Email, Name = model.AuthorName };
            var result = await _userManager.CreateAsync(user, model.LoginData.Password);

            if (!result.Succeeded)
            {
                //handle errors
            }

            if(model.IsAdmin == true)
            {
                var role = await _userContext.Roles.SingleOrDefaultAsync(r => r.Name == BlogConstants.AdministratorRoleName);
                var userRole = 
                await _userContext.UserRoles.AddAsync(
                    new IdentityUserRole<string>()
                    {
                        RoleId = role.Id,
                        UserId = user.Id });
                await _userContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [Route("/Delete{id}")]
        public async Task<IActionResult> Delete(string id)
        {

            var userToRemove = await _userContext.Users.SingleOrDefaultAsync(u => u.Id == id);
            var authorized = await _authorizationService.AuthorizeAsync(User, userToRemove, BlogAuthorization.Delete);
            if (authorized.Succeeded)
            {
                _userContext.Remove(userToRemove);
                await _userContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToReferer();
        }

        #region helpers

        [NonAction]
        bool UserIsAuthorized()
        {
            return User.IsInRole(BlogConstants.AdministratorRoleName);
        }

        [NonAction]
        IActionResult RedirectToReferer()
        {
            var referer = Request.Headers["Referer"].ToString();
            return Redirect(referer);
        }

        #endregion
    }
}