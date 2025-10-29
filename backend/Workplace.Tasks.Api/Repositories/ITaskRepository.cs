using Workplace.Tasks.Api.Models;

namespace Workplace.Tasks.Api.Repositories
{
    public interface ITaskRepository
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task<TaskEntity> AddAsync(TaskEntity task);
        Task<TaskEntity> UpdateAsync(TaskEntity task);
        Task DeleteAsync(Guid id);
    }
}
