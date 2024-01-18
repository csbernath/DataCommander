using System;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal static class ConnectionNameProvider
{
    public static string GetConnectionName(Func<SqlConnection> createConnection)
    {
        string serverVersion;
        string userName = null;
        SqlConnectionStringBuilder sqlConnectionStringBuilder;
        using (var connection = createConnection())
        {
            connection.Open();
            serverVersion = new Version(connection.ServerVersion).ToString();
            sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connection.ConnectionString);
            if (sqlConnectionStringBuilder.IntegratedSecurity)
            {
                var commanExecutor = connection.CreateCommandExecutor();
                const string commandText = "select suser_sname()";
                var createCommandRequest = new CreateCommandRequest(commandText);
                var scalar = commanExecutor.ExecuteScalar(createCommandRequest);
                userName = (string)scalar;
            }
        }

        if (!sqlConnectionStringBuilder.IntegratedSecurity)
            userName = sqlConnectionStringBuilder.UserID;

        return $"{sqlConnectionStringBuilder.DataSource}(SQL Server {serverVersion} - {userName})";
    }
}