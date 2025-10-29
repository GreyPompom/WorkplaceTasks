using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            return Ok(tasks);
        }

        // POST /api/tasks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskEntity task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("O título é obrigatório.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst(ClaimTypes.Name)?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized("Token inválido — ID de usuário não encontrado.");

            task.CreatedById = userId;

            var createdTask = await _taskService.CreateAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] TaskEntity updatedTask)
        {
            var existing = await _taskService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst(ClaimTypes.Name)?.Value;
            Guid.TryParse(userIdClaim, out var userId);

            // RBAC: Admin = tudo; Manager = tudo exceto deletar não próprias; Member = apenas próprias
            if (userRole == "Member" && existing.CreatedById != userId)
                return Forbid("Você só pode editar tarefas que criou.");

            existing.Title = updatedTask.Title;
            existing.Description = updatedTask.Description;
            existing.Status = updatedTask.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _taskService.UpdateAsync(existing);
            return Ok(result);
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
