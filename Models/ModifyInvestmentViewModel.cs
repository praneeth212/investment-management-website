using Managament.Models.Domain;

namespace Managament.Models
{
    public class ModifyInvestmentViewModel
    {
        public int InvestmentId { get; set; }
        public DateTime StartDate { get; set; }
        public InvestmentType InvestmentType { get; set; }
        public Frequency Frequency { get; set; }
    }
}