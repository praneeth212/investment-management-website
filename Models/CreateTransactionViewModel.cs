using System.ComponentModel.DataAnnotations;

namespace Managament.Models
{
    public class CreateTransactionViewModel
    {
        [Required]
        public int InvestmentId { get; set; }

        [Required]
        [Display(Name = "Mutual Fund")]
        public string MutualFundName { get; set; }

        [Required]
        [Display(Name = "Current NAV")]
        [Range(0.01, double.MaxValue, ErrorMessage = "NAV must be greater than 0.")]
        public decimal CurrentNav { get; set; }

        [Required]
        [Display(Name = "Amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; }
    }
}
