using System;
using System.Linq;
using Workplace.Tasks.Api.Models;
using Workplace.Tasks.Api.Data;
using BCrypt.Net;

namespace Workplace.Tasks.Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any())
                return; 

            var users = new[]
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin User",
                    Email = "admin@example.com",
                    Role = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Manager User",
                    Email = "manager@example.com",
                    Role = "Manager",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Member User",
                    Email = "member@example.com",
                    Role = "Member",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
            var tasks = new[]
           {
                new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Configuração inicial do sistema",
                    Description = "Revisar ambiente e garantir funcionamento do backend.",
                    Status = TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = admin.Id
                },
                new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Planejar sprints do projeto",
                    Description = "Definir backlog e planejar as primeiras entregas.",
                    Status = TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = manager.Id
                },
                new TaskEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Criar mockups de interface",
                    Description = "Desenhar telas iniciais para validação do design.",
                    Status = TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = member.Id
                }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }
    }
}
