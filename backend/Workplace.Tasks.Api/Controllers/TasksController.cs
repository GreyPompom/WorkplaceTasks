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
    public class TasksController : ControllerBase
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

        // GET /api/tasks
        [HttpGet]
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

        // POST /api/tasks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("O título é obrigatório.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst(ClaimTypes.Name)?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido — ID de usuário não encontrado.");

            var task = new TaskEntity
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                CreatedById = userId
            };

            var createdTask = await _taskService.CreateAsync(task);

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

        //  GET /api/tasks/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        //  PUT /api/tasks/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskUpdateDto dto)
        {
            var userId = GetUserIdFromClaims();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var existing = await _taskService.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Tarefa não encontrada.");

            if (role == "Member" && existing.CreatedById != userId)
                throw new UnauthorizedAccessException("Você só pode editar tarefas criadas por você.");

            existing.Title = dto.Title;
            existing.Description = dto.Description;
            existing.Status = dto.Status;

            var result = await _taskService.UpdateAsync(existing);

            //criar metodo ToReponse
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

        // DELETE /api/tasks/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _taskService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst(ClaimTypes.Name)?.Value;
            Guid.TryParse(userIdClaim, out var userId);

            if (userRole == "Member" && existing.CreatedById != userId)
                return Forbid("Você só pode excluir tarefas que criou.");

            if (userRole == "Manager" && existing.CreatedById != userId)
                return Forbid("Managers só podem excluir tarefas criadas por eles.");

            await _taskService.DeleteAsync(id);
            return NoContent();
        }
    }
}
