using Microsoft.EntityFrameworkCore;
using Workplace.Tasks.Api.Data;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Models.Enum;

namespace Workplace.Tasks.Api.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks.AsNoTracking().ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<TaskEntity> AddAsync(TaskEntity task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskEntity> UpdateAsync(TaskEntity task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _context.Tasks.FindAsync(id);
            if (existing != null)
            {
                _context.Tasks.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<TaskEntity> Items, int TotalCount)> GetPagedAsync(string? status, int pageNumber, int pageSize)
        {
            var query = _context.Tasks.AsQueryable();
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<EnumTaskStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(t => t.Status == parsedStatus);
            } //esse so funciona para banco postgress


            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
