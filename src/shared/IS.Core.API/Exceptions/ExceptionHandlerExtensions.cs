using Microsoft.Extensions.DependencyInjection;

namespace IS.Core.API.Exceptions
{
    public static class ExceptionHandlerExtensions
    {
        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        {
            services.AddProblemDetails()
                    .AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
