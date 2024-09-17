using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace DataCommander.Providers.SqlServer;

public static class SqlObjectScripter
{
    public static SqlConnectionInfo CreateSqlConnectionInfo(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var csb = new SqlConnectionStringBuilder(connectionStringAndCredential.ConnectionString);

        var connectionInfo = new SqlConnectionInfo
        {
            ApplicationName = csb.ApplicationName,
            ConnectionTimeout = csb.ConnectTimeout,
            DatabaseName = csb.InitialCatalog,
            EncryptConnection = csb.Encrypt,
            MaxPoolSize = csb.MaxPoolSize,
            MinPoolSize = csb.MinPoolSize,
            PacketSize = csb.PacketSize,
            Pooled = csb.Pooling,
            ServerName = csb.DataSource,
            UseIntegratedSecurity = csb.IntegratedSecurity,
            WorkstationId = csb.WorkstationID,
            TrustServerCertificate = csb.TrustServerCertificate
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