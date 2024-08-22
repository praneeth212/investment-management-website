using Managament.Data;
using Managament.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Management.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly MVCDemoDbContext _context;

        public TransactionRepository(MVCDemoDbContext context)
        {
            _context = context;
        }

        // Fetching latest transactions for a specific investment
        public async Task<Transaction> GetLatestTransactionAsync(int investmentId)
        {
            return await _context.Transactions
                .Where(t => t.InvestmentId == investmentId) // Filter transactions by investmentId
                .OrderByDescending(t => t.TransactionDate) // Order by TransactionDate in descending order
                .FirstOrDefaultAsync(); // Return the first transaction or null if no transactions exist
        }
    }
}