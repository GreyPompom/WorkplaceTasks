using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;

namespace Workplace.Tasks.Api.Middlewares
{
    public class ModelStateValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ModelStateValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Antes do controller executar, intercepta ModelState inválido
            context.Request.EnableBuffering();

            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var actionDescriptor = endpoint.Metadata
                    .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
                    .FirstOrDefault();

                if (actionDescriptor != null)
                {
                    var modelState = context.Features.Get<ModelStateFeature>()?.ModelState;
                    if (modelState != null && !modelState.IsValid)
                    {
                        var errors = modelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                            );

                        var problem = new ValidationProblemDetails(errors)
                        {
                            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                            Title = "One or more validation errors occurred.",
                            Status = StatusCodes.Status400BadRequest,
                            Instance = context.Request.Path
                        };

                        context.Items["ModelStateErrors"] = problem;
                        context.Response.StatusCode = 400;
                    }
                }
            }

            await _next(context);
        }
    }
}
