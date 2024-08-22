using Managament.Data;
using Managament.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Managament.Services
{
    public class AuthService : IAuthService
    {
        private readonly MVCDemoDbContext _context;

        public AuthService(MVCDemoDbContext context)
        {
            _context = context;
        }

        // Async method for validating user credentials
        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            // Fetching a customer where email matches the provided email
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);

            // If customer is found AND provided password matches the stored hashed password
            if (customer != null && BCrypt.Net.BCrypt.Verify(password, customer.Password))
            {
                return true;
            }
            return false;
        }
    }
}