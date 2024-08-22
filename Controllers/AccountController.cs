using System.Security.Claims;
using Managament.Data;
using Managament.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Management.Controllers
{
    public class AccountController : Controller
    {
        // Private field to access the database context.
        private readonly MVCDemoDbContext mVCDemoDbContext;

        // Initialize the database context
        public AccountController(MVCDemoDbContext mVCDemoDbContext)
        {
            this.mVCDemoDbContext = mVCDemoDbContext;
        }

        // GET: Display the login page
        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

        // POST: Handle login form submission
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Check if the provided model is valid (e.g: required fields are filled out).
            if (ModelState.IsValid)
            {
                // Attempt to find the customer in the database by email.
                var customer = await mVCDemoDbContext.Customers
                    .FirstOrDefaultAsync(c => c.Email == model.Email);

                // If the customer exists AND provided password matches the hashed password in the database.
                if (customer != null && BCrypt.Net.BCrypt.Verify(model.Password, customer.Password))
                {
                    // Creating a list of claims for authenticated user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, customer.Name),
                        new Claim(ClaimTypes.Email, customer.Email)
                    };

                    // Creating a claims identity with the specified authentication scheme.
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Sign in the user with the created claims principal.
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Setting Bootstrap Toast
                    TempData["SuccessMessage"] = "You have been logged in successfully.";
                    TempData["IsLoggedIn"] = true;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Setting Bootstrap Toast
                    ViewBag.IsLoggedIn = false;
                    TempData["ErrorMessage"] = "Invalid email or password.";
                }
            }
            // If model state is invalid or login fails, redisplay the login view with the provided model.
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Signing out the user from the cookie authentication scheme.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Setting Bootstrap Toast
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}