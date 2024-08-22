using Managament.Models.Domain;
using Management.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Management.Repositories
{
    public interface IInvestmentRepository
    {
        Task<IEnumerable<Investment>> GetActiveSIPInvestmentsAsync();
    }
}
