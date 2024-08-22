using Microsoft.AspNetCore.Mvc;

namespace Managment.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new HomePageViewModel
            {
                MutualFundsInfo = "Mutual funds pool money from many investors to purchase securities. They offer diversification and professional management.",
                SipInfo = "SIPs allow you to invest a fixed amount in mutual funds at regular intervals, providing the benefit of rupee cost averaging and disciplined investing.",
                BenefitsOfInvesting = new List<string>
                {
                    "Potential for higher returns compared to traditional savings methods.",
                    "Diversification reduces risk.",
                    "Liquidity: Easy to buy and sell."
                }
            };

            return View(viewModel);
        }
    }

    public class HomePageViewModel
    {
        public string MutualFundsInfo { get; set; }
        public string SipInfo { get; set; }
        public List<string> BenefitsOfInvesting { get; set; }
    }
}
