using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Workplace.Tasks.Api.Services;

namespace Workplace.Tasks.Api.Authorization
{
    public class OwnsTaskRequirement : IAuthorizationRequirement { }

    public class OwnsTaskHandler : AuthorizationHandler<OwnsTaskRequirement>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OwnsTaskHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnsTaskRequirement requirement)
        {
            // id do user no jwt
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return;

            var userId = Guid.Parse(userIdClaim);

            var httpContext = context.Resource switch
            {
                HttpContext ctx => ctx,
                Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext filterCtx => filterCtx.HttpContext,
                _ => null
            };

            if (httpContext == null)
                return;

            if (!httpContext.Request.RouteValues.TryGetValue("id", out var idObj))
                return;

            if (!Guid.TryParse(idObj?.ToString(), out var taskId))
                return;

            using var scope = _scopeFactory.CreateScope();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
            var task = await taskService.GetByIdAsync(taskId);

            if (task == null)
                return;

            //  pode tudo
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            //  manager pode editar/deletar apenas próprias
            if (context.User.IsInRole("Manager") && task.CreatedById == userId)
            {
                context.Succeed(requirement);
                return;
            }

            // member idem
            if (context.User.IsInRole("Member") && task.CreatedById == userId)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
