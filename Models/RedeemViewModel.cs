namespace Managament.Models
{
    public class RedeemViewModel
    {
        public int InvestmentId { get; set; }
        public decimal UnitsOwned { get; set; }
        public decimal ExitLoadPercentage { get; set; }
        public decimal UnitsToRedeem { get; set; }
        public decimal LatestNavValue { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}