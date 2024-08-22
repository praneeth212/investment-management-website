namespace Managament.Models.Domain
{
    public class NAV
    {
        public int NavId { get; set; }
        public int MutualFundId { get; set; }
        public decimal NAVValue { get; set; }
        public DateTime NAVDate { get; set; }

        public MutualFund MutualFund { get; set; }
    }
}
