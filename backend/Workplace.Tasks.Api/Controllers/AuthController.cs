using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Services;
using Workplace.Tasks.Api.DTOs.Auth;
using Workplace.Tasks.Api.DTOs.User;

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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
           
                var user = await _userService.RegisterAsync(dto);

                // gera token 
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["Jwt:Key"];
                var key = Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("Chave JWT não configurada."));

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.Name)
                };

                var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = creds
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwt = tokenHandler.WriteToken(token);

                var response = new AuthResponseDto
                {
                    Token = jwt,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role
                    }
                };

                return Ok(response);
           
        }


        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] LoginRequestDto dto)
        {
           
                var user = await _userService.ValidateCredentialsAsync(dto.Email, dto.Password);

                if (user == null)
                    throw new UnauthorizedAccessException("Email ou senha inválidos.");

                // cria a chave jwt
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration["Jwt:Key"];
                var key = Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("Chave JWT não configurada."));

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.Name)
                };

                var creds = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                );

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = creds
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwt = tokenHandler.WriteToken(token);

                var response = new AuthResponseDto
                {
                    Token = jwt,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role
                    }
                };

                return Ok(response);
           
        }

    }
}
