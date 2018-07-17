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
        public async Task<IActionResult> Add(AccountViewModel model)
        {
            if (UserIsAuthorized())
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                return RedirectToAction("Index");
            }
            return RedirectToReferer();
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
            return User.IsInRole(BlogConstants.AdministratorRole);
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