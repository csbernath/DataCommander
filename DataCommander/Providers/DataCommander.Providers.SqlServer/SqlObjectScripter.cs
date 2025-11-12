using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace DataCommander.Providers.SqlServer;

public static class SqlObjectScripter
{
    public static SqlConnectionInfo CreateSqlConnectionInfo(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionStringAndCredential.ConnectionString);

        var connectionInfo = new SqlConnectionInfo
        {
            ApplicationName = sqlConnectionStringBuilder.ApplicationName,
            ConnectionTimeout = sqlConnectionStringBuilder.ConnectTimeout,
            DatabaseName = sqlConnectionStringBuilder.InitialCatalog,
            EncryptConnection = sqlConnectionStringBuilder.Encrypt,
            MaxPoolSize = sqlConnectionStringBuilder.MaxPoolSize,
            MinPoolSize = sqlConnectionStringBuilder.MinPoolSize,
            PacketSize = sqlConnectionStringBuilder.PacketSize,
            Pooled = sqlConnectionStringBuilder.Pooling,
            ServerName = sqlConnectionStringBuilder.DataSource,
            TrustServerCertificate = sqlConnectionStringBuilder.TrustServerCertificate,            
            UseIntegratedSecurity = sqlConnectionStringBuilder.IntegratedSecurity,
            WorkstationId = sqlConnectionStringBuilder.WorkstationID
        };

        var credential = connectionStringAndCredential.Credential;
        if (credential != null)
        {
            connectionInfo.UserName = credential.UserId;
            connectionInfo.SecurePassword = credential.Password.SecureString;
        }

        return connectionInfo;
    }
}