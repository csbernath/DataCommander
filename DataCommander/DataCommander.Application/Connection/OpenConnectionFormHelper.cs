using System.Text;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Application.Connection;

internal static class OpenConnectionFormHelper
{
    public static string CreateOpenConnectionFormText(ConnectionInfo connectionInfo, ProviderInfo providerInfo, IProvider provider)
    {
        var connectionStringBuilder = provider.CreateConnectionStringBuilder();
        connectionStringBuilder.ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString;
        var dataSource = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSourceObject)
            ? (string?)dataSourceObject
            : null;
        var host = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.Host, out var hostObject)
            ? (string?)hostObject
            : null;
        var initialCatalog = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.InitialCatalog, out var initialCatalogObject)
            ? (string?)initialCatalogObject
            : null;
        var database = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.Database, out var databaseObject)
            ? (string?)databaseObject
            : null;
        bool? integratedSecurity = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var integratedSecurityObject)
            ? (bool)integratedSecurityObject!
            : null;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($@"Opening connection...

Connection name: {connectionInfo.ConnectionName}
Provider name: {providerInfo.Name}");
        
        if (dataSource != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.DataSource}: {dataSource}");
        else if (host != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.Host}: {host}");

        if (initialCatalog != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.InitialCatalog}: {initialCatalog}");
        else if (database != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.Database}: {database}");
        
        if (integratedSecurity == true)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
        
        var credential = connectionInfo.ConnectionStringAndCredential.Credential;
        if (credential != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {credential.UserId}");
        
        var text = stringBuilder.ToString();
        return text;
    }
}