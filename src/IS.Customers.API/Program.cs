using IS.Core.API.Exceptions;
using IS.Core.Logging;
using IS.Core.Mediator.Configuration;
using IS.Customers.API.Configuration;
using IS.Customers.API.Data;
using IS.Customers.API.Features.CreateCustomer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
                .AddGlobalExceptionHandler()
                .AddDbContext<CustomerDbContext>("PostgresConnection", builder.Environment)
                .AddDependenciesConfiguration()
                .AddMediator(typeof(CreateCustomerCommand).Assembly)
                .AddControllers();

builder.AddDefaultSerilog("LogsConnection");

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseOpenApiScalar();

app.UseExceptionHandler()
   .UseHttpsRedirection()
   .UseAuthorization()
   .UseSerilog();

app.MapControllers();
app.Run();
