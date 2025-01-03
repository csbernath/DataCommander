using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace DataCommander.Providers.SqlServer;

public static class SqlObjectScripter
{
    public static SqlConnectionInfo CreateSqlConnectionInfo(string connectionString)
    {
        var csb = new SqlConnectionStringBuilder(connectionString);

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
        if (!csb.IntegratedSecurity)
        {
            connectionInfo.UserName = csb.UserID;
            connectionInfo.Password = csb.Password;
        }

        return connectionInfo;
    }
}