using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace IS.Core.Logging
{
    public static class LogExtensions
    {
        public static WebApplicationBuilder AddDefaultSerilog(this WebApplicationBuilder builder, string connectionStringId)
        {
            IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                ["Message"] = new RenderedMessageColumnWriter(NpgsqlDbType.Text),
                ["MessageTemplate"] = new MessageTemplateColumnWriter(NpgsqlDbType.Text),
                ["Level"] = new LevelColumnWriter(renderAsText: true, NpgsqlDbType.Varchar),
                ["RaiseDate"] = new UtcTimestampColumnWriter(),
                ["Exception"] = new ExceptionColumnWriter(NpgsqlDbType.Text),
                ["Properties"] = new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb),
                ["SourceContext"] = new SinglePropertyColumnWriter(propertyName: "SourceContext", writeMethod: PropertyWriteMethod.ToString, dbType: NpgsqlDbType.Text),
                ["Application"] = new SinglePropertyColumnWriter(propertyName: "Application", writeMethod: PropertyWriteMethod.ToString, dbType: NpgsqlDbType.Varchar)
            };

            builder.Services.AddSerilog((services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                    .ReadFrom.Services(services)
                    .WriteTo.Console()
                    .WriteTo.PostgreSQL(
                        connectionString: builder.Configuration.GetConnectionString(connectionStringId),
                        tableName: "Logs",
                        columnOptions: columnWriters,
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        schemaName: "public",
                        needAutoCreateTable: true,
                        respectCase: true);
            });

            return builder;
        }

        public static IApplicationBuilder UseSerilog(this IApplicationBuilder webApplication)
        {
            webApplication.UseSerilogRequestLogging();

            return webApplication;
        }
    }
}
