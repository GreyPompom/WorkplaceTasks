using System;
using Workplace.Tasks.Api.DTOs.User;

namespace Workplace.Tasks.Api.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDTO User { get; set; } = new();
    }
}
