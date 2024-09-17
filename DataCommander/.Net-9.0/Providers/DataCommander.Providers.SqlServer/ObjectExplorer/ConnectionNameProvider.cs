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
        SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
        dataSource = sqlConnectionStringBuilder.DataSource;
        bool integratedSecurity = sqlConnectionStringBuilder.IntegratedSecurity;
        if (!integratedSecurity)
        {
            userId = connection.Credential != null
                ? connection.Credential.UserId
                : sqlConnectionStringBuilder.UserID;
        }

        serverVersion = connection.ServerVersion;
        if (integratedSecurity)
        {
            IDbCommandExecutor commanExecutor = connection.CreateCommandExecutor();
            const string commandText = "select suser_sname()";
            CreateCommandRequest createCommandRequest = new CreateCommandRequest(commandText);
            object scalar = commanExecutor.ExecuteScalar(createCommandRequest);
            userId = (string)scalar;
        }

        return $"{dataSource}(SQL Server {serverVersion} - {userId})";
    }
}