using Managament.Data;
using Managament.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Management.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        // Dependency Injection of the database context
        private readonly MVCDemoDbContext _context;

        // Injecting database context into repository
        public InvestmentRepository(MVCDemoDbContext context)
        {
            _context = context; // assign injected context to a private field
        }

        // Fetching active sip investments from the database
        public async Task<IEnumerable<Investment>> GetActiveSIPInvestmentsAsync()
        {
            // 1. IsActive is true
            // 2. InvestmentType is SIP
            return await _context.Investments
                .Where(i => i.IsActive && i.InvestmentType == InvestmentType.SIP)
                .Include(i => i.Customer) // eagerly loads related customer entity
                .ToListAsync(); // asynchronously converts the result to a list and returns it
        }
    }
}