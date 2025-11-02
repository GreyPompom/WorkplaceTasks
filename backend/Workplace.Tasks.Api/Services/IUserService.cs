using Workplace.Tasks.Api.DTOs.Auth;
using Workplace.Tasks.Api.DTOs.User;
using Workplace.Tasks.Api.Models;

namespace Workplace.Tasks.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user, string password);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<User?> ValidateCredentialsAsync(string email, string password);
        Task<User> RegisterAsync(RegisterRequestDto dto);
        Task<(IEnumerable<UserResponseDTO> Users, int TotalCount)> GetPagedAsync(int page, int pageSize, string? role, string? search);

    }
}
