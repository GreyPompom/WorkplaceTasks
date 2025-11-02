using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Workplace.Tasks.Api.DTOs;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Services;
using BCrypt.Net;
using Workplace.Tasks.Api.DTOs.User;

namespace Workplace.Tasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminPolicy")] // apenas admin acessa
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // get api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            var result = users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            });
            return Ok(result);
        }

        //post api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = dto.Role
            };

            var created = await _userService.CreateAsync(user, dto.Password);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }

        // update api/users/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDTO dto)
        {
            var existing = await _userService.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException("Usuário não encontrado.");

            if (existing.Role == "Admin")
                return BadRequest("Não é permitido alterar outro administrador.");

            existing.Name = dto.Name;
            existing.Role = dto.Role;

            var updated = await _userService.UpdateAsync(existing);
            return Ok(updated);
        }

        // delete api/users/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetByIdAsync(id)
                       ?? throw new KeyNotFoundException("Usuário não encontrado.");

            if (user.Role == "Admin")
                return BadRequest("Não é permitido excluir um administrador.");

            await _userService.DeleteAsync(id);
            return NoContent();
        }

        //get user pagination
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
                                          [FromQuery] string? role = null, [FromQuery] string? search = null)
        {
            var (users, totalCount) = await _userService.GetPagedAsync(page, pageSize, role, search);


            var result = new
            {
                data =users,
                totalCount
            };

            return Ok(result);
        }
    }
}
