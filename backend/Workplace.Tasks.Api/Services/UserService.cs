using Microsoft.EntityFrameworkCore;
using Workplace.Tasks.Api.Data;
using Workplace.Tasks.Api.Models;
using BCrypt.Net;
using Workplace.Tasks.Api.DTOs.Auth;

namespace Workplace.Tasks.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email)
                ?? throw new InvalidOperationException("Usuário não encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Senha incorreta.");

            return user;
        }
        public async Task<User> RegisterAsync(RegisterRequestDto dto)
        {
            //ve se o email ja é usado
            var existingUser = await GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("O e-mail informado já está em uso.");

            // hash senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Member" : dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

    }
}
