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
        }
    }
}
