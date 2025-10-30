using Workplace.Tasks.Api.DTOs.Auth;
using Workplace.Tasks.Api.Models;

namespace Workplace.Tasks.Api.Services
{
    public interface IUserService
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user, string password);
        Task<User?> ValidateCredentialsAsync(string email, string password);
        Task<User> RegisterAsync(RegisterRequestDto dto);
    }
}
