using Managament.Models.Domain;
using Managament.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Managament.Data;

namespace Managament.Controllers
{
    public class InvestmentsController : Controller
    {
        private readonly MVCDemoDbContext _context;
        private readonly ILogger<InvestmentsController> _logger;

        public InvestmentsController(MVCDemoDbContext context, ILogger<InvestmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Investments/Index
        public async Task<IActionResult> Index()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email found in user claims.");
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            if (customer == null)
            {
                _logger.LogWarning($"Customer with email {email} not found.");
                return RedirectToAction("Login", "Account");
            }

            var investments = await _context.Investments
                .Where(i => i.CustomerId == customer.Id)
                .Include(i => i.MutualFund)
                .ToListAsync();

            foreach (var investment in investments)
            {
                if (investment.InvestmentId == 0)
                {
                    _logger.LogWarning($"Investment with ID 0 found for customer {customer.Id}.");
                }
            }


            var viewModel = investments.Select(i => new InvestmentViewModel
            {
                InvestmentId = i.InvestmentId,
                MutualFundName = i.MutualFund.Name,
                StartDate = i.StartDate,
                AmountInvested = i.AmountInvested,
                UnitsOwned = i.UnitsOwned,
                InvestmentType = i.InvestmentType,
                IsActive = i.IsActive
            }).ToList();

            return View(viewModel);
        }


        // GET: Investments/Create
        public async Task<IActionResult> Create(int mutualFundId)
        {
            // Log the received ID for debugging
            _logger.LogInformation($"Received mutualFundId: {mutualFundId}");

            if (mutualFundId <= 0)
            {
                _logger.LogWarning($"Invalid mutual fund ID: {mutualFundId}.");
                return RedirectToAction("Error", "Home");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("No email found in user claims.");
                return RedirectToAction("Login", "Account");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            var mutualFund = await _context.MutualFunds
                .FirstOrDefaultAsync(mf => mf.MutualFundId == mutualFundId);

            if (customer == null || mutualFund == null)
            {
                _logger.LogWarning($"Customer or mutual fund not found. Customer: {customer?.Id}, MutualFund: {mutualFundId}");
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new InvestmentTransactionViewModel
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                MutualFundId = mutualFundId,
                MutualFundName = mutualFund.Name,
                StartDate = DateTime.Now,
                InvestmentType = InvestmentType.Lumpsum,
                Frequency = Frequency.Monthly
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvestmentTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Log all validation errors
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        _logger.LogWarning($"Model state error: {modelState.Key} - {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            if (model.CustomerId <= 0)
            {
                _logger.LogWarning($"Invalid CustomerId: {model.CustomerId}");
                return RedirectToAction("Login", "Account");
            }

            var mutualFund = await _context.MutualFunds
                .FirstOrDefaultAsync(mf => mf.MutualFundId == model.MutualFundId);

            if (mutualFund == null)
            {
                _logger.LogWarning($"Mutual fund not found for ID: {model.MutualFundId}");
                return RedirectToAction("Error", "Home");
            }

            var investment = new Investment
            {
                CustomerId = model.CustomerId,
                MutualFundId = model.MutualFundId,
                StartDate = model.StartDate,
                InvestmentType = model.InvestmentType,
                Frequency = model.Frequency,
                IsActive = true
            };

            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Investment created with ID: {investment.InvestmentId}");

            return RedirectToAction("AddTransaction", new { investmentId = investment.InvestmentId });
        }

        // GET: Investments/Modify/5
        public async Task<IActionResult> Modify(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid investment ID for modify action.");
                return RedirectToAction("Index");
            }

            var investment = await _context.Investments
                .FirstOrDefaultAsync(i => i.InvestmentId == id);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {id}");
                return RedirectToAction("Index");
            }

            var viewModel = new ModifyInvestmentViewModel
            {
                InvestmentId = investment.InvestmentId,
                StartDate = investment.StartDate,
                InvestmentType = investment.InvestmentType,
                Frequency = investment.Frequency
            };

            return View(viewModel);
        }


        // POST: Investments/Modify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(ModifyInvestmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Log model state errors
                _logger.LogWarning("Model state is invalid. Model state errors: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            var investment = await _context.Investments
                .FirstOrDefaultAsync(i => i.InvestmentId == model.InvestmentId);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {model.InvestmentId}");
                return RedirectToAction("Index");
            }

            // Update properties
            investment.StartDate = model.StartDate;
            investment.InvestmentType = model.InvestmentType;
            investment.Frequency = model.Frequency;

            // Explicitly set the entity state to modified
            _context.Entry(investment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Investment updated successfully.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Concurrency error while updating investment: {ex.Message}");
                return View("Error");
            }

            return RedirectToAction("Index");
        }




        // GET: Investments/AddTransaction
        public async Task<IActionResult> AddTransaction(int investmentId)
        {
            var investment = await _context.Investments
                .Include(i => i.MutualFund)
                .FirstOrDefaultAsync(i => i.InvestmentId == investmentId);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {investmentId}");
                return RedirectToAction("Error", "Home");
            }

            var latestNav = await _context.NAVs
                .Where(nav => nav.MutualFundId == investment.MutualFundId)
                .OrderByDescending(nav => nav.NAVDate)
                .FirstOrDefaultAsync();

            if (latestNav == null)
            {
                _logger.LogWarning($"No NAV data found for MutualFund ID: {investment.MutualFundId}");
                return RedirectToAction("Error", "Home");
            }

            var viewModel = new CreateTransactionViewModel
            {
                InvestmentId = investment.InvestmentId,
                MutualFundName = investment.MutualFund.Name,
                CurrentNav = latestNav.NAVValue,
                Amount = 0,
                TransactionType = "Buy"
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(CreateTransactionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Log all validation errors
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        _logger.LogWarning($"Model state error: {modelState.Key} - {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            var investment = await _context.Investments
                .Include(i => i.MutualFund)
                .FirstOrDefaultAsync(i => i.InvestmentId == model.InvestmentId);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {model.InvestmentId}");
                return RedirectToAction("Error", "Home");
            }

            var transaction = new Transaction
            {
                InvestmentId = model.InvestmentId,
                Units = model.Amount / model.CurrentNav,
                Amount = model.Amount,
                TransactionDate = DateTime.Now,
                TransactionType = model.TransactionType
            };

            investment.AmountInvested += model.Amount;
            investment.UnitsOwned += transaction.Units;

            _context.Transactions.Add(transaction);
            _context.Investments.Update(investment);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transaction added for Investment ID: {model.InvestmentId}");

            return RedirectToAction(nameof(Index));
        }

        // POST: Investments/Cancel
        // Cancelling/Pausing/Resuming Investment functionalities
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int investmentId)
        {
            var investment = await _context.Investments.FindAsync(investmentId);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {investmentId}");
                return RedirectToAction("Error", "Home");
            }

            // Set investment to inactive
            investment.IsActive = false;

            _context.Investments.Update(investment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Investment ID {investmentId} has been paused.");

            return RedirectToAction(nameof(Index));
        }

        // POST: Investments/Resume/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resume(int investmentId)
        {
            var investment = await _context.Investments.FindAsync(investmentId);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {investmentId}");
                return RedirectToAction("Error", "Home");
            }

            // Set investment to active
            investment.IsActive = true;

            _context.Investments.Update(investment);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Investment ID {investmentId} has been resumed.");

            return RedirectToAction(nameof(Index));
        }

        //Redeeming 
        // Redeeming 
        public async Task<IActionResult> Redeem(int investmentId)
        {
            // Fetch the investment data along with related mutual fund data
            var investment = await _context.Investments
                .Include(i => i.MutualFund) // Include mutual fund to get exit load percentage
                .FirstOrDefaultAsync(i => i.InvestmentId == investmentId);

            // Handle case where investment is not found
            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {investmentId}");
                return RedirectToAction("Error", "Home");
            }

            // Fetch the latest NAV value for the mutual fund
            var latestNav = await _context.NAVs
                .Where(n => n.MutualFundId == investment.MutualFundId)
                .OrderByDescending(n => n.NAVDate)
                .FirstOrDefaultAsync();

            // Fetch the oldest transaction date for the investment
            var oldestTransaction = await _context.Transactions
                .Where(t => t.InvestmentId == investmentId)
                .OrderBy(t => t.TransactionDate)
                .FirstOrDefaultAsync();

            // Prepare the view model with necessary data
            var viewModel = new RedeemViewModel
            {
                InvestmentId = investment.InvestmentId,
                UnitsOwned = investment.UnitsOwned,
                ExitLoadPercentage = investment.MutualFund.ExitLoad, // Directly access ExitLoad
                LatestNavValue = latestNav?.NAVValue ?? 0, // Fetch latest NAV value
                TransactionDate = oldestTransaction?.TransactionDate ?? DateTime.Now // Fetch the oldest transaction date
            };

            // Return the view with the view model
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Redeem(RedeemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Log all validation errors
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        _logger.LogWarning($"Model state error: {modelState.Key} - {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            // Fetch the investment data along with related mutual fund data
            var investment = await _context.Investments
                .Include(i => i.MutualFund)
                .FirstOrDefaultAsync(i => i.InvestmentId == model.InvestmentId);

            if (investment == null)
            {
                _logger.LogWarning($"Investment not found for ID: {model.InvestmentId}");
                return RedirectToAction("Error", "Home");
            }

            if (investment.InvestmentType == InvestmentType.SIP && investment.IsActive)
            {
                _logger.LogWarning($"SIP must be paused or cancelled to redeem for Investment ID: {model.InvestmentId}");
                ModelState.AddModelError("", "SIP must be paused or cancelled to redeem.");
                return View(model);
            }

            // Fetch transactions and sort by date
            var transactions = await _context.Transactions
                .Where(t => t.InvestmentId == model.InvestmentId)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            // Fetch the latest NAV value
            var latestNav = await _context.NAVs
                .Where(n => n.MutualFundId == investment.MutualFundId)
                .OrderByDescending(n => n.NAVDate)
                .FirstOrDefaultAsync();

            decimal latestNavValue = latestNav?.NAVValue ?? 0;

            // Log latest NAV and units to redeem
            _logger.LogInformation($"Latest NAV Value: {latestNavValue}");
            _logger.LogInformation($"Units to Redeem: {model.UnitsToRedeem}");

            // Check if NAV value is zero
            if (latestNavValue == 0)
            {
                _logger.LogWarning("Latest NAV Value is zero. Redemption cannot proceed.");
                ModelState.AddModelError("", "Redemption cannot proceed because the NAV value is zero.");
                return View(model);
            }

            decimal redeemValue = 0;
            decimal unitsToRedeem = model.UnitsToRedeem;

            foreach (var transaction in transactions)
            {
                if (unitsToRedeem <= 0)
                    break;

                var redeemUnits = Math.Min(transaction.Units, unitsToRedeem);
                unitsToRedeem -= redeemUnits;

                var transactionDate = transaction.TransactionDate;
                var currentDate = DateTime.Now;
                var daysElapsed = (currentDate - transactionDate).Days;
                var exitLoadApplicable = daysElapsed < 365;
                var exitLoadRate = investment.MutualFund.ExitLoad;

                // Calculate redemption value before applying exit load
                redeemValue += redeemUnits * latestNavValue;

                // Apply exit load if applicable (subtract from total redemption value)
                if (exitLoadApplicable)
                {
                    redeemValue -= redeemUnits * latestNavValue * (exitLoadRate / 100);
                }

                _logger.LogInformation($"Transaction Date: {transactionDate}, Units: {redeemUnits}, Redeem Value: {redeemValue}");

                // Update the transaction to reflect redeemed units
                transaction.Units -= redeemUnits;
                _context.Transactions.Update(transaction);
            }

            if (unitsToRedeem > 0)
            {
                _logger.LogWarning("Units to redeem exceed available units.");
                ModelState.AddModelError("", "Redemption units exceed available units.");
                return View(model);
            }

            // Update investment details
            investment.UnitsOwned -= model.UnitsToRedeem;
            investment.AmountInvested -= redeemValue;
            investment.AmountRedeemed += redeemValue;

            _logger.LogInformation($"Investment updated - Units Owned: {investment.UnitsOwned}, Amount Invested: {investment.AmountInvested}, Amount Redeemed: {investment.AmountRedeemed}");

            // Save the redemption transaction
            var redeemTransaction = new Transaction
            {
                InvestmentId = model.InvestmentId,
                Units = -model.UnitsToRedeem,
                Amount = -redeemValue,
                TransactionDate = DateTime.Now,
                TransactionType = "Redemption"
            };

            _logger.LogInformation($"Redeem Transaction - Units: {redeemTransaction.Units}, Amount: {redeemTransaction.Amount}");

            _context.Transactions.Add(redeemTransaction);
            _context.Investments.Update(investment);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Redemption processed for Investment ID: {model.InvestmentId}");

            return RedirectToAction(nameof(Index));
        }



    }
}
