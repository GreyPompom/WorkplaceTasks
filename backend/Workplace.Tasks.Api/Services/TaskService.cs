using Workplace.Tasks.Api.DTOs;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Repositories;

namespace Workplace.Tasks.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _taskRepository.GetAllAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<TaskEntity> CreateAsync(TaskEntity task)
        {
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            return await _taskRepository.AddAsync(task);
        }

        public async Task<TaskEntity> UpdateAsync(TaskEntity task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            return await _taskRepository.UpdateAsync(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _taskRepository.DeleteAsync(id);
        }

        //paginação
        public async Task<PagedResponseDto<TaskEntity>> GetPagedAsync(TaskFilterDto filter)
        {
            var (items, totalCount) = await _taskRepository.GetPagedAsync(filter.Status, filter.PageNumber, filter.PageSize);

            return new PagedResponseDto<TaskEntity>
            {
                Items = items,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
