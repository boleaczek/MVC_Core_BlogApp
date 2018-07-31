using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Security;
using Blog.Models;
using Blog.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.UnitsOfWork;

namespace Blog.Controllers.AdminPanel
{
    [Route("/AdminPanel/UserManager")]
    [Authorize]
    public class UserManagerController : Controller
    {
        private readonly IAccountUnitOfWork _accountUnitOfWork;
        private readonly ISecurityFacade _securityFacade;

        public UserManagerController(IAccountUnitOfWork accountUnitOfWork, ISecurityFacade securityFacade)
        {
            _accountUnitOfWork = accountUnitOfWork;
            _securityFacade = securityFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (UserIsAuthorized())
            {
                ICollection<ApplicationUser> users = await _accountUnitOfWork.Users.GetAll().ToListAsync();
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

            var user = new ApplicationUser { UserName = model.LoginData.Email, Email = model.LoginData.Email, Name = model.AuthorName};
            
            await _accountUnitOfWork.Users.Insert(user, model.LoginData.Password);

            if(model.IsAdmin == true)
            {
                var role = await _accountUnitOfWork.Roles.SearchFor(r => r.Name == BlogConstants.AdministratorRoleName).SingleOrDefaultAsync();
                
                _accountUnitOfWork.UserRoles.Insert(
                    new IdentityUserRole<string>()
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    });
                await _accountUnitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        [Route("/Delete{id}")]
        public async Task<IActionResult> Delete(string id)
        {

            var userToRemove = await _accountUnitOfWork.Users.GetById(id);
            var authorized = await _securityFacade.IsAuthorized(userToRemove, BlogConstants.DeleteActionName);

            if (authorized)
            {
                await _accountUnitOfWork.Users.Delete(userToRemove);
                return RedirectToAction("Index");
            }

            return RedirectToReferer();
        }

        #region helpers

        [NonAction]
        bool UserIsAuthorized()
        {
            return _securityFacade.IsInRole(BlogConstants.AdministratorRoleName);
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