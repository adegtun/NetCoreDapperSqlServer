using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Models;

namespace TestWebApp.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        private Users GetUserByUsername(string username)
        {
            // TODO: Replace this:
            Users retValue = null;
            if (username == "Me")
            {
                retValue = new Users()
                {
                    Username = "Me",
                    Pwd = "1",
                    FirstName = "Myself",
                    LastName = "AndI"
                };
                retValue.Roles.Add("Admin");
            }
            return retValue;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Find and validate the user:
            Users user = GetUserByUsername(username);
            if (user == null || user.Pwd != password)
            {
                ModelState.AddModelError("", "Incorrect username or password.");
                return View();
            }

            // Create the identity
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));

            // Add roles
            foreach (var role in user.Roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            // Sign in
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}