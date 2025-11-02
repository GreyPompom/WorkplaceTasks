using Workplace.Tasks.Api.Models;

namespace Workplace.Tasks.Api.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task SaveChangesAsync();
        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(int page, int pageSize, string? role, string? search);
    }
}
