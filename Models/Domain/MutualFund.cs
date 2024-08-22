using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Managament.Models.Domain
{
    public enum MutualFundType
    {
        Equity,
        Debt,
        Hybrid
    }

    public enum RiskAssessment
    {
        High,
        Moderate,
        Low
    }
    public class MutualFund
    {
        public int MutualFundId { get; set; }
        public float MinimumInvestment { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public MutualFundType Type { get; set; }
        public decimal ExitLoad { get; set; }
        public RiskAssessment Risk { get; set; }

        public ICollection<Investment> Investments { get; set; } = new List<Investment>();
        public ICollection<NAV> NAVs { get; set; } = new List<NAV>();
    }
}
