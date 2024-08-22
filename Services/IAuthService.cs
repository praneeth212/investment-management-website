using System.Threading.Tasks;

public interface IAuthService
{
    Task<bool> ValidateUserAsync(string email, string password);
}
