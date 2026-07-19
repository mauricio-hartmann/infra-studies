using Microsoft.OpenApi;

namespace IS.Ticket.API.Configuration;

public static class OpenApiConfig
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services, OpenApiSpecVersion openApiSpecVersion = OpenApiSpecVersion.OpenApi3_0)
    {
        services.AddOpenApi(options => options.OpenApiVersion = openApiSpecVersion);

        return services;
    }

    public static WebApplication UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
        }

        return app;
    }
}