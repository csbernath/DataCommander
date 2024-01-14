using System;
using System.Security;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal static class ConnectionNameProvider
{
    public static string GetConnectionName(string connectionString, SecureString? password)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
        
        string serverVersion;
        string userName = null;
        using (var connection = new Connection(connectionString, password))
        {
            connection.Connection.Open();
            serverVersion = new Version(connection.ServerVersion).ToString();

            if (connectionStringBuilder.IntegratedSecurity)
            {
                var commanExecutor = connection.Connection.CreateCommandExecutor();
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