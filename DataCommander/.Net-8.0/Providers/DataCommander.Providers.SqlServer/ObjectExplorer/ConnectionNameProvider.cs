using System;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal static class ConnectionNameProvider
{
    public static string GetConnectionName(string connectionString)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

        string serverVersion;
        string userName = null;
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            serverVersion = new Version(connection.ServerVersion).ToString();

            if (connectionStringBuilder.IntegratedSecurity)
            {
                var commanExecutor = connection.CreateCommandExecutor();
                const string commandText = "select suser_sname()";
                var createCommandRequest = new CreateCommandRequest(commandText);
                var scalar = commanExecutor.ExecuteScalar(createCommandRequest);
                userName = (string)scalar;
            }
        }

        if (!connectionStringBuilder.IntegratedSecurity)
            userName = connectionStringBuilder.UserID;

        return $"{connectionStringBuilder.DataSource}(SQL Server {serverVersion} - {userName})";
    }
}