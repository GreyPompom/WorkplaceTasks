using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Workplace.Tasks.Api.DTOs;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Services;


namespace Workplace.Tasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : BaseController
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
           
        // Criar, listar, atualizar e excluir tarefas; - feito testado
        // Usar o CreatedById do token JWT; - feito testad
        // Fazer validações básicas(campos obrigatórios e permissões); - feito
        // Garantir segurança com[Authorize]. feito

        public TasksController(ITaskService taskService, IUserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        // GET /api/tasks - Regra: Todos (Admin, Manager, Member) podem listar
        [HttpGet]
        [Authorize(Policy = "MemberPolicy")]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await _taskService.GetAllAsync();
            var response = tasks.Select(tasks => new TaskResponseDto
            {
                Id = tasks.Id,
                Title = tasks.Title,
                Description = tasks.Description,
                Status = tasks.Status,
                CreatedAt = tasks.CreatedAt,
                UpdatedAt = tasks.UpdatedAt,
                CreatedById = tasks.CreatedById
            });
            return Ok(response);
        }

        // GET /api/tasks/{id}  - Regra: Todos (Admin, Manager, Member) podem listar
        [HttpGet("{id:guid}")] 
        [Authorize(Policy = "MemberPolicy")] 
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException("Tarefa não encontrada.");

            return Ok(task);
        }

        // POST /api/tasks - todos
        [HttpPost]
        [Authorize(Policy = "MemberPolicy")]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            var userId = GetUserId();
            var newTask = new TaskEntity
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                CreatedById = userId
            };

            var createdTask = await _taskService.CreateAsync(newTask);

            var response = new TaskResponseDto
            {
                Id = createdTask.Id,
                Title = createdTask.Title,
                Description = createdTask.Description,
                Status = createdTask.Status,
                CreatedAt = createdTask.CreatedAt,
                UpdatedAt = createdTask.UpdatedAt,
                CreatedById = createdTask.CreatedById
            };
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }


        //  PUT /api/tasks/{id} - todos, mas member que so pode editar as proprias
        [HttpPut("{id:guid}")]
        [Authorize(Policy = "OwnsTask")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskUpdateDto dto)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var existing = await _taskService.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Tarefa não encontrada.");

            if (role == "Member" && existing.CreatedById != userId)
                return Forbid();

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Status = dto.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _taskService.UpdateAsync(existing);

            //Melhoria criar metodo ToReponse
            var response = new TaskResponseDto
            {
                Id = result.Id,
                Title = result.Title,
                Description = result.Description,
                Status = result.Status,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt,
                CreatedById = result.CreatedById
            };
            return Ok(response);
        }

        // DELETE /api/tasks/{id} - todos, mas member e manager so pode deletar as proprias
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "OwnsTask")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var existing = await _taskService.GetByIdAsync(id)
                            ?? throw new KeyNotFoundException("Tarefa não encontrada.");

            //regra para member e manager
            if (role == "Member" && existing.CreatedById != userId)
                return Forbid();

            if (role == "Manager" && existing.CreatedById != userId)
                return Forbid();

            await _taskService.DeleteAsync(id);
            return NoContent();
        }
    }
}
