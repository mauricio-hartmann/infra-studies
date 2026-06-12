using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace IS.Customers.API.Configuration
{
    public static class OpenApiConfig
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services, OpenApiSpecVersion openApiSpecVersion = OpenApiSpecVersion.OpenApi3_0)
        {
            services.AddOpenApi(options => options.OpenApiVersion = openApiSpecVersion);

            return services;
        }

        public static WebApplication UseOpenApiScalar(this WebApplication app)
        {
            app.MapOpenApi();
            app.MapScalarApiReference();

            return app;
        }
    }
}
