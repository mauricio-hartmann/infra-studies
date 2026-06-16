using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IS.Core.API.Exceptions
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);

            int statusCode = (int)HttpStatusCode.InternalServerError;
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = "Internal server error",
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };
            problem.Extensions.TryAdd("traceId", httpContext.TraceIdentifier);
            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}
