using IS.Customers.API.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
                .AddControllers();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseOpenApiScalar();

app.UseHttpsRedirection()
    .UseAuthorization();

app.MapControllers();
app.Run();