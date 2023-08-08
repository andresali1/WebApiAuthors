using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiAuthors.Middlewares
{
    public static class ResponseHttpLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseHttpLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ResponseHttpLogMiddleware>();
        }
    }

    public class ResponseHttpLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseHttpLogMiddleware> _logger;

        public ResponseHttpLogMiddleware(RequestDelegate next, ILogger<ResponseHttpLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var originalBodyResponse = context.Response.Body;
                context.Response.Body = ms;

                await _next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string response = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(originalBodyResponse);
                context.Response.Body = originalBodyResponse;

                _logger.LogInformation(response);
            }
        }
    }
}
