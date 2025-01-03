using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal static class ConnectionNameProvider
{
    public static string? GetConnectionName(SqlConnection connection)
    {
        string dataSource;
        string? serverVersion;
        string? userId = null;
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
        dataSource = sqlConnectionStringBuilder.DataSource;
        var integratedSecurity = sqlConnectionStringBuilder.IntegratedSecurity;
        if (!integratedSecurity)
        {
            userId = connection.Credential != null
                ? connection.Credential.UserId
                : sqlConnectionStringBuilder.UserID;
        }

        serverVersion = connection.ServerVersion;
        if (integratedSecurity)
        {
            var commanExecutor = connection.CreateCommandExecutor();
            const string commandText = "select suser_sname()";
            var createCommandRequest = new CreateCommandRequest(commandText);
            var scalar = commanExecutor.ExecuteScalar(createCommandRequest);
            userId = (string)scalar!;
        }

        return $"{dataSource}(SQL Server {serverVersion} - {userId})";
    }
}