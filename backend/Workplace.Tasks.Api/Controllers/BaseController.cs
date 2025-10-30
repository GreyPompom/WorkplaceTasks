using Microsoft.AspNetCore.Mvc;
using Workplace.Tasks.Api.Models;
using System.Security.Claims;


namespace Workplace.Tasks.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        //Centraliza validações repetidas

        /// <summary>
        /// Retorna o ID do usuário autenticado (Claim: NameIdentifier).
        /// </summary>
        protected Guid GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idClaim))
                throw new UnauthorizedAccessException("Token inválido — ID de usuário não encontrado.");

            return Guid.Parse(idClaim);
        }

        /// <summary>
        /// Retorna a role do usuário autenticado (Claim: Role).
        /// </summary>
        protected string GetUserRole()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(role))
                throw new UnauthorizedAccessException("Role não encontrada no token.");

            return role;
        }

        /// <summary>
        /// Verifica se o usuário é Admin.
        /// </summary>
        protected bool IsAdmin() => GetUserRole() == "Admin";

        /// <summary>
        /// Verifica se o usuário é Manager.
        /// </summary>
        protected bool IsManager() => GetUserRole() == "Manager";
        /// <summary>
        /// Verifica se o usuário é Member.
        /// </summary>
        protected bool IsMember() => GetUserRole() == "Member";

        /// <summary>
        /// Retorna informações básicas do usuário autenticado.
        /// </summary>
        protected (Guid UserId, string Role, string? Email, string? Name) GetUserInfo()
        {
            return (
                UserId: GetUserId(),
                Role: GetUserRole(),
                Email: User.FindFirst(ClaimTypes.Email)?.Value,
                Name: User.FindFirst(ClaimTypes.Name)?.Value
            );
        }

        //MElhoria criar POLICIES cutomizadas direto no program.cs
    }

}
