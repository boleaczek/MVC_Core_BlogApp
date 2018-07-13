using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        UserContext _userContext;

        public UserManagerController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, UserContext userContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ICollection<ApplicationUser> users = await _userContext.Users.ToListAsync();
            return View(new UserManagerViewModel() { Users = users });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AccountViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            return RedirectToAction("Index");
        }

        [Route("/Delete{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userContext.Users.SingleOrDefaultAsync(u => u.Id == id);
            _userContext.Remove(user);
            await _userContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}