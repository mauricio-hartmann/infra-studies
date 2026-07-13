using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace IS.Core.HealthCheck;

public sealed class PostgreSqlConnectionHealthCheck : IHealthCheck
{
    private const string DefaultTestQuery = "Select 1";

    private readonly string _connectionString;
    private readonly string _testQuery;

    public PostgreSqlConnectionHealthCheck(string connectionString) : this(connectionString, testQuery: DefaultTestQuery)
    {
    }

    public PostgreSqlConnectionHealthCheck(string connectionString, string testQuery)
    {
        _connectionString = connectionString;
        _testQuery = testQuery;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = _testQuery;

            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("PostgreSQL is available.");
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "PostgreSQL is unavailable.", exception);
        }
    }
}
