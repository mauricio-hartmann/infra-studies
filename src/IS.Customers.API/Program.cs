using IS.Core.Logging;
using IS.Customers.API.Configuration;
using IS.Customers.API.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
                .AddDbContext<CustomerDbContext>("PostgresConnection", builder.Environment)
                .AddDependenciesConfiguration()
                .AddControllers();

builder.AddDefaultSerilog("LogsConnection");

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseOpenApiScalar();

app.UseHttpsRedirection()
   .UseAuthorization()
   .UseSerilog();

app.MapControllers();
app.Run();
