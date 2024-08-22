using Microsoft.AspNetCore.Mvc;
using Managament.Data;
using Managament.Models.Domain;
using System.Linq;
using Managament.Models;

namespace Managament.Controllers
{
    public class MutualFundsController : Controller
    {
        private readonly MVCDemoDbContext _context;

        public MutualFundsController(MVCDemoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string riskAssessment, string sortOrder)
        {
            // Retrieve the mutual funds
            var mutualFunds = from mf in _context.MutualFunds
                              select mf;

            // Apply filtering
            if (!string.IsNullOrEmpty(riskAssessment))
            {
                if (Enum.TryParse(riskAssessment, true, out RiskAssessment parsedRisk))
                {
                    mutualFunds = mutualFunds.Where(mf => mf.Risk == parsedRisk);
                }
            }

            // Apply sorting
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.RiskSortParm = sortOrder == "Risk" ? "risk_desc" : "Risk";
            ViewBag.SelectedRiskAssessment = riskAssessment;

            switch (sortOrder)
            {
                case "name_desc":
                    mutualFunds = mutualFunds.OrderByDescending(mf => mf.Name);
                    break;
                case "Risk":
                    mutualFunds = mutualFunds.OrderBy(mf => mf.Risk);
                    break;
                case "risk_desc":
                    mutualFunds = mutualFunds.OrderByDescending(mf => mf.Risk);
                    break;
                default:
                    mutualFunds = mutualFunds.OrderBy(mf => mf.Name);
                    break;
            }

            return View(mutualFunds.ToList());
        }

        public IActionResult Details(int id)
        {
            var mutualFund = _context.MutualFunds
                .FirstOrDefault(mf => mf.MutualFundId == id);

            if (mutualFund == null)
            {
                return NotFound();
            }

            var latestNav = _context.NAVs
                .Where(nav => nav.MutualFundId == id)
                .OrderByDescending(nav => nav.NAVDate)
                .Select(nav => nav.NAVValue)
                .FirstOrDefault();

            var viewModel = new MutualFundDetailsViewModel
            {
                MutualFund = mutualFund,
                LatestNav = latestNav
            };

            return View(viewModel);
        }


    }
}
