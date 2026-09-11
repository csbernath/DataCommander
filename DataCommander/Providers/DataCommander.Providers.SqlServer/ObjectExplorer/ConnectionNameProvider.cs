using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal static class ConnectionNameProvider
{
    public static string GetConnectionName(string? connectionName, SqlConnection connection)
    {
        string? serverVersion;
        string? userId = null;
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);

        if (string.IsNullOrEmpty(connectionName))
            connectionName = sqlConnectionStringBuilder.DataSource;
        
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

        return $"{connectionName} (SQL Server {serverVersion} - {userId})";
    }
}