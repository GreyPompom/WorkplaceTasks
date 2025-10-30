using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Workplace.Tasks.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Captura erros de ModelState automaticamente
                context.Response.OnStarting(() =>
                {
                    if (context.Items.ContainsKey("ModelStateErrors"))
                    {
                        var problem = (ValidationProblemDetails)context.Items["ModelStateErrors"]!;
                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = problem.Status ?? 400;
                        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                        return context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
                    }

                    return Task.CompletedTask;
                });

                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            var problem = new ValidationProblemDetails(
                ex.Errors.GroupBy(e => e.PropertyName)
                         .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            )
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Status = (int)HttpStatusCode.BadRequest,
                Instance = context.Request.Path
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status.Value;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "Erro interno ao processar a solicitação.",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.InternalServerError,
                Instance = context.Request.Path
            };

            if (ex is UnauthorizedAccessException)
            {
                problem.Status = (int)HttpStatusCode.Unauthorized;
                problem.Title = "Acesso negado.";
            }
            else if (ex is KeyNotFoundException)
            {
                problem.Status = (int)HttpStatusCode.NotFound;
                problem.Title = "Recurso não encontrado.";
            }
            else if (ex is InvalidOperationException)
            {
                problem.Status = (int)HttpStatusCode.BadRequest;
                problem.Title = "Requisição inválida.";
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? 500;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
        }
    }
}
