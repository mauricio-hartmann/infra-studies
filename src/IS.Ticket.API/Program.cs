using IS.Core.API.Exceptions;
using IS.Core.Cache.Configuration;
using IS.Core.Data.Configuration;
using IS.Core.Logging;
using IS.Ticket.API.Configuration;
using IS.Ticket.API.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi()
                .AddGlobalExceptionHandler()
                .AddDbContext<TicketDbContext>("PostgresTicketsConnection", builder.Environment)
                .AddDependenciesConfiguration()
                .AddCache(builder.Configuration, "RedisConnection")
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