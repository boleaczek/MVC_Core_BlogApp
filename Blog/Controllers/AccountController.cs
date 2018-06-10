using Blog.Models;
using Blog.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            //var user = new ApplicationUser { UserName = "admin", Email = "admin@blog.com" };
            //var result = await _userManager.CreateAsync(user, "Admin_panel1");
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, false);
            result = await _signInManager.PasswordSignInAsync("admin", loginViewModel.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("", "AdminPanel");
            }
            return RedirectToAction("","Home");
        }
    }
}
