namespace Managament.Models.Domain
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int InvestmentId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal Units { get; set; }
        public DateTime TransactionDate { get; set; }
        public Investment Investment { get; set; }
    }
}
