using IS.Core.API.Exceptions;
using IS.Core.Cache.Configuration;
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
                .AddCache(builder.Configuration, "RedisConnection", "IS.Customer.API")
                .AddHealthChecksConfiguration(builder.Configuration)
                .AddControllers();

builder.AddDefaultSerilog("LogsConnection");

WebApplication app = builder.Build();

app.UseOpenApi()
   .MapHealthChecks()
   .UseExceptionHandler()
   .UseHttpsRedirection()
   .UseAuthorization()
   .UseSerilog();

app.MapControllers();
app.Run();
