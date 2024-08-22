namespace Managament.Models.Domain
{
    public enum InvestmentType
    {
        Lumpsum,
        SIP
    }

    public enum Frequency
    {
        Daily,
        Monthly,
        Quarterly
    }

    public class Investment
    {
        public int InvestmentId { get; set; }
        public bool IsActive { get; set; }
        public int CustomerId { get; set; }
        public int MutualFundId { get; set; }
        public DateTime StartDate { get; set; }
        public decimal AmountInvested { get; set; }
        public decimal UnitsOwned { get; set; }
        public Frequency Frequency { get; set; }
        public decimal AmountRedeemed { get; set; }
        public InvestmentType InvestmentType { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public MutualFund MutualFund { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
