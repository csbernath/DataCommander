using System.Text;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Application.Connection;

internal sealed class OpenConnectionFormHelper
{
    public static string CreateOpenConnectionFormText(ConnectionInfo connectionInfo, ProviderInfo providerInfo, IProvider provider)
    {
        var connectionStringBuilder = provider.CreateConnectionStringBuilder();
        connectionStringBuilder.ConnectionString = connectionInfo.ConnectionStringAndCredential.ConnectionString;
        var dataSource = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSourceObject)
            ? (string)dataSourceObject
            : null;
        var host = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.Host, out var hostObject)
            ? (string)hostObject
            : null;
        var containsIntegratedSecurity = connectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out var integratedSecurity);
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($@"Connection name: {connectionInfo.ConnectionName}
Provider name: {providerInfo.Name}");
        if (dataSource != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.DataSource}: {dataSource}");
        else if (host != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.Host}: {host}");
        if (containsIntegratedSecurity)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.IntegratedSecurity}: {integratedSecurity}");
        var credential = connectionInfo.ConnectionStringAndCredential.Credential;
        if (credential != null)
            stringBuilder.Append($"\r\n{ConnectionStringKeyword.UserId}: {credential.UserId}");
        var text = stringBuilder.ToString();
        return text;
    }
}