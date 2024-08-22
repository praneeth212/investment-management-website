using Managament.Data;
using Managament.Models;
using Management.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace Managament.Controllers
{
    public class FundController : Controller
    {
        private readonly MVCDemoDbContext _context;

        public FundController(MVCDemoDbContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            var funds = _context.MutualFunds.ToList();

            // Provide initial data (e.g., for the first fund or a default fund)
            var initialFundId = funds.FirstOrDefault()?.MutualFundId ?? 0;
            var initialData = GetNAVData(initialFundId);
            ViewBag.DataPoints = JsonConvert.SerializeObject(initialData);

            var viewModel = new HomeViewModel
            {
                Funds = funds
            };

            return View(viewModel);
        }
        public JsonResult GetFundData(int fundId)
        {
            var dataPoints = GetNAVData(fundId);
            return Json(dataPoints);
        }

        private List<DataPoint> GetNAVData(int MutualFundId)
        {
            return _context.NAVs
                .Where(nav => nav.MutualFundId == MutualFundId)
                .OrderBy(nav => nav.NAVDate)
                .Select(nav => new DataPoint(nav.NAVDate.ToUnixTimeMilliseconds(), (double)nav.NAVValue))
                .ToList();
        }
    }
}
