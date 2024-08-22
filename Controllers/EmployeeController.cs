using Managament.Data;
using Managament.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Managament.Models;
using Management.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Management.Controllers
{
    public class EmployeeController : Controller
    {
        // To access Database context
        private readonly MVCDemoDbContext mvcDemoDbContext;


        public EmployeeController(MVCDemoDbContext mvcDemoDbContext)
        {
            this.mvcDemoDbContext = mvcDemoDbContext;
        }

        // GET: Displaying the form to Register
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: Handling the submission of Register form
        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {
            // Checking if a customer already exists with that email
            var existingCustomer = await mvcDemoDbContext.Customers
                .FirstOrDefaultAsync(c => c.Email == addEmployeeRequest.Email);

            // If customer exists with that email, redirect to same page with Toast
            if (existingCustomer != null)
            {
                TempData["ErrorMessage"] = "Email is already used by another account.";
                return RedirectToAction("Add", "Employee");
            }

            // Create new customer object and populating data
            var customer = new Customer
            {
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Phone = addEmployeeRequest.Phone,
                Pan_no = addEmployeeRequest.Pan_no,
                // Hashing password using Bcrypt before storing in database
                Password = BCrypt.Net.BCrypt.HashPassword(addEmployeeRequest.Password),
                Address = addEmployeeRequest.Address
            };

            // Adding a new customer to database and save changes
            await mvcDemoDbContext.Customers.AddAsync(customer);
            await mvcDemoDbContext.SaveChangesAsync();

            // Setting Bootstrap Toast & Redirect to login page
            TempData["SuccessMessage"] = "You have successfully registered. Please Login.";
            return RedirectToAction("Login", "Account");
        }


        // GET: Displaying the customer's profile details
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Get the currently logged-in user's mail from the claims
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Find the customer in database using his email
            var employee = await mvcDemoDbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);

            // If customer is found, populate the view model with their details and return profile
            if (employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    Address = employee.Address,
                    Pan_no = employee.Pan_no
                };
                return await Task.Run(() => View("Profile", (viewModel)));
            }
            // If customer is not found, redirect to profile section.
            // When customer is not logged in, redirect to login page.
            return RedirectToAction("Profile", "Employee");
        }

        // POST: Handling profile updates by the user
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateEmployeeViewModel model)
        {
            // Finding the user in the database with his id from view model
            var employee = await mvcDemoDbContext.Customers.FindAsync(model.Id);

            if (employee != null)
            {
                try
                {
                    // Updating the user's details with the data from the view model
                    employee.Name = model.Name;
                    employee.Email = model.Email;
                    employee.Phone = model.Phone;
                    employee.Address = model.Address;
                    employee.Pan_no = model.Pan_no;

                    // Mark the user's entity as modified and save changes
                    mvcDemoDbContext.Customers.Update(employee);
                    await mvcDemoDbContext.SaveChangesAsync();

                    // Setting Bootstrap toast
                    TempData["SuccessMessage"] = "Your profile has been updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating your profile. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Employee not found.";
            }

            // After updating details, redirect the user to home page
            return RedirectToAction("Index", "Home");
        }

        // GET: Displaying Forgot password view where reset link can be triggered
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Handling the form submission for requesting a password reset link
        [HttpPost]
        public async Task<IActionResult> SendPasswordResetLink(Customer model)
        {
            // Finding the customer by email
            var employee = await mvcDemoDbContext.Customers.FirstOrDefaultAsync(c => c.Email == model.Email);
            if (employee != null)
            {
                try
                {
                    // Generating a secure random reset token
                    var resetToken = GenerateResetToken();

                    // Storing the reset token and expiry time in the database
                    employee.PasswordResetToken = resetToken;
                    employee.PasswordResetTokenExpiry = DateTime.Now.AddHours(1); // Token valid for 1 hour
                    await mvcDemoDbContext.SaveChangesAsync();

                    // Sending email with reset link
                    var resetLink = Url.Action("ResetPassword", "Employee", new { token = resetToken }, Request.Scheme);
                    SendResetLinkEmail(employee.Email, resetLink);

                    // Notify user that reset link has been sent using toast
                    TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
                }
                catch (Exception ex)
                {
                    // Handling any errors that occur during process
                    TempData["ErrorMessage"] = "An error occurred while sending the password reset link. Please try again.";
                }
            }
            else
            {
                // Handling and Notifying user that 'Email not found' in database
                TempData["ErrorMessage"] = "Email not found.";
            }

            return RedirectToAction("ForgotPassword");
        }



        // GET: Displays the Reset Password view where users can reset using the provided token
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            // Finding the customer by reset token and check if the token is valid
            var employee = await mvcDemoDbContext.Customers.FirstOrDefaultAsync(c => c.PasswordResetToken == token && c.PasswordResetTokenExpiry > DateTime.Now);
            if (employee != null)
            {
                // Creating a view model for the Reset Password view with the token
                var model = new ResetPasswordViewModel { Token = token };
                return View(model);
            }

            // Returning a NotFound result if the token is invalid or expired
            return NotFound("Invalid or expired token.");
        }

        // POST: Handles the form submission for resetting the password.
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // Validate the model state.
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find the customer by reset token and check if the token is valid.
            var employee = await mvcDemoDbContext.Customers.FirstOrDefaultAsync(c => c.PasswordResetToken == model.Token && c.PasswordResetTokenExpiry > DateTime.Now);

            if (employee != null)
            {
                // Update the customer's password and clear the reset token.
                employee.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                employee.PasswordResetToken = null;
                employee.PasswordResetTokenExpiry = null;
                await mvcDemoDbContext.SaveChangesAsync();

                // Notify the user that the password has been reset successfully.
                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToAction("Login", "Account");
            }

            // Notify the user if the token is invalid or expired.
            TempData["ErrorMessage"] = "Invalid or expired token.";
            return View(model);
        }

        // Sends the password reset link to the user's email.
        private void SendResetLinkEmail(string email, string resetLink)
        {
            var fromEmail = "vinaysriramtummidi01@gmail.com";
            var fromPassword = "drjm pkfi bvuy gbsi";

            // Defining sender and recipient email addresses
            var fromAddress = new MailAddress(fromEmail, "ManagementApp");
            var toAddress = new MailAddress(email);

            // Configuring SMTP client settings
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            // Creating email message
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Password Reset",
                Body = $"Click <a href=\"{resetLink}\">here</a> to reset your password.",
                IsBodyHtml = true
            })
            {
                // Sending email
                smtp.Send(message);
            }
        }

        // Method to generate a secure random token
        private static string GenerateResetToken(int length = 32)
        {
            // Creating a new instance of RNGCryptoServiceProvider to generate secure random bytes
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Creating an array and filling it with random bytes
                var byteArray = new byte[length];
                rng.GetBytes(byteArray);

                // Convert the byte array to a base64 string with more modifications
                return Convert.ToBase64String(byteArray).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }
    }
}