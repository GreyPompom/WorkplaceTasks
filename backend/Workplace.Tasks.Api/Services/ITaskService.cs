using Workplace.Tasks.Api.DTOs;
using Workplace.Tasks.Api.Models;

namespace Workplace.Tasks.Api.Services
{
    public interface ITaskService
    {
        Task<List<TaskEntity>> GetAllAsync();
        Task<TaskEntity?> GetByIdAsync(Guid id);
        Task<TaskEntity> CreateAsync(TaskEntity task);
        Task<TaskEntity> UpdateAsync(TaskEntity task);
        Task DeleteAsync(Guid id);

        //paginação
        Task<PagedResponseDto<TaskEntity>> GetPagedAsync(TaskFilterDto filter);

    }
}
