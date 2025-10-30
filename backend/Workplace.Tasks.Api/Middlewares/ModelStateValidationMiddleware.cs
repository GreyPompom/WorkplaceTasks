using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;


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
            context.Request.EnableBuffering();

            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                newBody.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(newBody).ReadToEndAsync();

                var problem = new
                {
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "Erro de validação",
                    status = 400,
                    errors = JsonSerializer.Deserialize<object>(body)
                };

                var json = JsonSerializer.Serialize(problem);
                context.Response.ContentType = "application/problem+json";
                context.Response.ContentLength = json.Length;

                newBody.SetLength(0);
                await context.Response.WriteAsync(json);
            }

            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }
    }
    public static class ModelStateValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseModelStateValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ModelStateValidationMiddleware>();
        }
    }
}
