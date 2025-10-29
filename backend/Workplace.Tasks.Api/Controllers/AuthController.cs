using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Services;

namespace Workplace.Tasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existingUser = await _userService.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("E-mail já cadastrado.");

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                Role = request.Role
            };

            await _userService.CreateAsync(newUser, request.Password);
            return Ok(new { message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginRequest request)
        {
            var isValid = await _userService.ValidateCredentialsAsync(request.Email, request.Password);
            if (!isValid)
                return Unauthorized("Credenciais inválidas.");

            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("name", user.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                user = new { user.Id, user.Name, user.Email, user.Role }
            });
        }
    }

    public record RegisterRequest(string Name, string Email, string Password, string Role);
    public record LoginRequest(string Email, string Password);
}
