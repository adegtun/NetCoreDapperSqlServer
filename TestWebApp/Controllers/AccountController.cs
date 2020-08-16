using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestWebApp.Models;
using TestWebApp.Utility;

namespace TestWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private MyConnection con;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            con = new MyConnection(_configuration);
        }

        //private Users GetUserByUsername(string username)
        //{
        //    // TODO: Replace this:
        //    Users retValue = null;
        //    if (username == "Me")
        //    {
        //        retValue = new Users()
        //        {
        //            Username = "Me",
        //            Pwd = "1",
        //            FirstName = "Myself",
        //            LastName = "AndI"
        //        };
        //        retValue.Roles.Add("Admin");
        //    }
        //    return retValue;
        //}

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Find and validate the user:
            //Users user = GetUserByUsername(username);
            User user = null;
            string query = "select * from users where username = @Username and password = @Password";
            IDbConnection dbcon = con.GetConnection();
            try
            {
                var rs = await dbcon.QueryAsync<User>(query, new { Username = username, Password = password }).ConfigureAwait(false);
                 user = rs.FirstOrDefault();
            }
            catch (Exception ex)
            {
                //_logger.Error(ex.ToString());
            }
            finally
            {
                dbcon.Close();
                dbcon.Dispose();
            }
            if (user == null)
            {
                ModelState.AddModelError("", "Incorrect username or password.");
                return View();
            }

            // Create the identity
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));

            //// Add roles
            //foreach (var role in user.Roles)
            //{
            //    identity.AddClaim(new Claim(ClaimTypes.Role, role));
            //}

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