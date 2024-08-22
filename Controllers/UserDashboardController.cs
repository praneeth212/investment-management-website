using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Managament.Data;
using Managament.Models.Domain;
using Managament.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace Managament.Controllers
{
    public class UserDashboardController : Controller
    {
        private readonly MVCDemoDbContext _context; // Database context for interacting with the database
        private readonly ILogger<UserDashboardController> _logger; // Logger for logging information and warnings

        // Constructor to initialize the context and logger
        public UserDashboardController(MVCDemoDbContext context, ILogger<UserDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: UserDashboard/Index
        // Action method to display the user's dashboard
        public async Task<IActionResult> Index()
        {
            // Get the email of the currently logged-in user
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Check if the email is found in the user claims
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email found in user claims."); // Log a warning if email is not found
                return RedirectToAction("Error", "Home"); // Redirect to error page if email is not found
            }

            // Fetch the customer based on the email
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            // Check if the customer exists
            if (customer == null)
            {
                _logger.LogWarning($"Customer with email {email} not found."); // Log a warning if customer is not found
                return RedirectToAction("Error", "Home"); // Redirect to error page if customer is not found
            }

            // Fetch the investments associated with the customer
            var investments = await _context.Investments
                .Where(i => i.CustomerId == customer.Id)
                .Include(i => i.MutualFund) // Include mutual fund details for each investment
                .ToListAsync();

            // Fetch the most recent 10 transactions for the customer's investments
            var transactions = await _context.Transactions
                .Where(t => investments.Select(i => i.InvestmentId).Contains(t.InvestmentId))
                .OrderByDescending(t => t.TransactionDate) // Order transactions by date descending
                .Take(10) // Take the most recent 10 transactions
                .ToListAsync();

            // Calculate the total amount invested by the customer
            var investedAmount = investments.Sum(i => i.AmountInvested);

            // Calculate the current amount based on the latest NAV values
            var currentAmount = investments.Sum(i => i.UnitsOwned * _context.NAVs
                .Where(nav => nav.MutualFundId == i.MutualFundId)
                .OrderByDescending(nav => nav.NAVDate)
                .FirstOrDefault().NAVValue);

            // Calculate the profit or loss
            var profitLoss = currentAmount - investedAmount;

            // Create and populate the view model with the necessary data
            var viewModel = new UserDashboardViewModel
            {
                RecentTransactions = transactions,
                InvestedAmount = investedAmount,
                CurrentAmount = currentAmount,
                ProfitLoss = profitLoss
            };

            // Return the view with the populated view model
            return View(viewModel);
        }
    }
}
