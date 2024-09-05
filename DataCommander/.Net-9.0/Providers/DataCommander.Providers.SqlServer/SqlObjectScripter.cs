using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace DataCommander.Providers.SqlServer;

public static class SqlObjectScripter
{
    public static SqlConnectionInfo CreateSqlConnectionInfo(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var csb = new SqlConnectionStringBuilder(connectionStringAndCredential.ConnectionString);

        var connectionInfo = new SqlConnectionInfo();
        connectionInfo.ApplicationName = csb.ApplicationName;
        connectionInfo.ConnectionTimeout = csb.ConnectTimeout;
        connectionInfo.DatabaseName = csb.InitialCatalog;
        connectionInfo.EncryptConnection = csb.Encrypt;
        connectionInfo.MaxPoolSize = csb.MaxPoolSize;
        connectionInfo.MinPoolSize = csb.MinPoolSize;
        connectionInfo.PacketSize = csb.PacketSize;
        connectionInfo.Pooled = csb.Pooling;
        connectionInfo.ServerName = csb.DataSource;
        connectionInfo.UseIntegratedSecurity = csb.IntegratedSecurity;
        connectionInfo.WorkstationId = csb.WorkstationID;
        connectionInfo.TrustServerCertificate = csb.TrustServerCertificate;

        var credential = connectionStringAndCredential.Credential;
        if (credential != null)
        {
            connectionInfo.UserName = credential.UserId;
            connectionInfo.SecurePassword = credential.Password.SecureString;
        }

        return connectionInfo;
    }
}