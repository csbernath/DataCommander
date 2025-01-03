using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Core;

namespace DataCommander.Application.Connection;

public sealed class ConnectionProperties
{
    public readonly string ConnectionName;
    public readonly string ProviderName;

    public Option<string> Password;
    public IProvider Provider;
    public string ConnectionString;
    public ConnectionBase Connection;

    public ConnectionProperties(string connectionName, string providerName, IProvider provider)
    {
        ConnectionName = connectionName;
        ProviderName = providerName;
        Provider = provider;
    }
}