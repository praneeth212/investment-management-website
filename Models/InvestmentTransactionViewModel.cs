using Managament.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Managament.Models
{
    public class InvestmentTransactionViewModel
    {
        [Required]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "The Customer Name field is required.")]
        public string CustomerName { get; set; }

        [Required]
        public int MutualFundId { get; set; }

        [Required]
        public string MutualFundName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public InvestmentType InvestmentType { get; set; }

        [Required]
        public Frequency Frequency { get; set; }
    }
}