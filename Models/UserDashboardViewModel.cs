using Managament.Models.Domain;
using System.Collections.Generic;

namespace Managament.Models
{
    public class UserDashboardViewModel
    {
        public IEnumerable<Transaction> RecentTransactions { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}