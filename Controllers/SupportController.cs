using Microsoft.AspNetCore.Mvc;
using Management.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Managament.Models;
using Managament.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Management.Controllers
{
    public class SupportController : Controller
    {
        private readonly MVCDemoDbContext mVCDemoDbContext;

        // Constructor that takes the MVCDemoDbContext as a dependency
        public SupportController(MVCDemoDbContext mVCDemoDbContext)
        {
            this.mVCDemoDbContext = mVCDemoDbContext;
        }

        // GET: Support/Create
        [Authorize] // Ensure the user is authenticated
        [HttpGet] // This action handles GET requests
        public async Task<IActionResult> Create()
        {
            // Retrieve the current user's email from the claims
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Fetch the customer details from the database based on the user's email
            var customer = await mVCDemoDbContext.Customers
                .SingleOrDefaultAsync(c => c.Email == userEmail);

            // If no customer is found, redirect to an error page
            if (customer == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Create a new SupportRequestViewModel and set the CustomerId
            var model = new SupportRequestViewModel
            {
                CustomerId = customer.Id
            };

            // Return the view with the model
            return View(model);
        }

        // POST: Support/Create
        [Authorize] // Ensure the user is authenticated
        [HttpPost] // This action handles POST requests
        public async Task<IActionResult> Create(SupportRequestViewModel model)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create a new SupportRequest object from the model
                var supportRequest = new SupportRequest
                {
                    CustomerId = model.CustomerId,
                    Subject = model.Subject,
                    Message = model.Message,
                    CreatedAt = DateTime.UtcNow
                };

                // Add the support request to the database
                mVCDemoDbContext.SupportRequests.Add(supportRequest);

                // Save changes to the database
                await mVCDemoDbContext.SaveChangesAsync();

                // Set a success message in TempData
                TempData["SuccessMessage"] = "Your support request has been sent successfully!";

                // Redirect to the home page
                return RedirectToAction("Index", "Home");
            }

            // If the model state is not valid, return the view with the model
            return View(model);
        }
    }
}
