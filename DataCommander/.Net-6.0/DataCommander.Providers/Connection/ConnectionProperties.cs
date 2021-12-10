using DataCommander.Providers2;
using DataCommander.Providers2.Connection;
using Foundation.Core;

namespace DataCommander.Providers.Connection;

public sealed class ConnectionProperties
{
    public readonly string ConnectionName;
    public readonly string ProviderName;

    public Option<string> Password;
    public IProvider Provider;
    public string ConnectionString;
    public ConnectionBase Connection;

    public ConnectionProperties(string connectionName, string providerName)
    {
        ConnectionName = connectionName;
        ProviderName = providerName;
    }
}