using Microsoft.EntityFrameworkCore;
using Workplace.Tasks.Api.Data;
using Workplace.Tasks.Api.Models;
using BCrypt.Net;
using Workplace.Tasks.Api.DTOs.Auth;
using Workplace.Tasks.Api.DTOs.User;
using Workplace.Tasks.Api.Repositories;

namespace Workplace.Tasks.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            //Melhoria Criar rotina de primeiro acesso
            //if (password == null) {
            //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("senhaPadrao");
            //}
            //else
            //{
            //    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            //}

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
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

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();


            return user;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("Usuário não encontrado.");

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<(IEnumerable<UserResponseDTO> Users, int TotalCount)> GetPagedAsync(int page, int pageSize, string? role, string? search)
        {
            var (users, totalCount) = await _userRepository.GetPagedAsync(page, pageSize, role, search);

            var userDtos = users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            });

            return (userDtos, totalCount);
        }
    }
}
