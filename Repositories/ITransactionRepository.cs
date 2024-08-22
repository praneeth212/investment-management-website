using Managament.Models.Domain;
using System.Threading.Tasks;

namespace Management.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetLatestTransactionAsync(int investmentId);
    }
}
