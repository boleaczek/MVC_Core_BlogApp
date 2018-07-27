using Blog.Models;
using Blog.Models.ViewModels;
using Blog.Security;
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
        readonly ISecurityFacade _securityFacade;

        public AccountController(ISecurityFacade securityFacade)
        {
            _securityFacade = securityFacade;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginViewModel loginViewModel)
        {
            var result = await _securityFacade.LogIn(loginViewModel.Email, loginViewModel.Password);

            if (result)
            {
                return RedirectToAction("", "AdminPanel");
            }

            return RedirectToAction("","Home");
        }
    }
}
