using Managament.Models.Domain;

namespace Managament.Models
{
    public class InvestmentViewModel
    {
        public int InvestmentId { get; set; }
        public string MutualFundName { get; set; }
        public DateTime StartDate { get; set; }
        public decimal AmountInvested { get; set; }
        public decimal UnitsOwned { get; set; }
        public InvestmentType InvestmentType { get; set; }
        public bool IsActive { get; set; }
    }

}
