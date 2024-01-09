using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Core;

namespace DataCommander.Application.Connection;

public sealed class ConnectionProperties(string connectionName, string providerIdentifier, IProvider? provider)
{
    public readonly string ConnectionName = connectionName;
    public readonly string ProviderIdentifier = providerIdentifier;

    public Option<string> Password;
    public IProvider? Provider = provider;
    public string ConnectionString;
    public ConnectionBase Connection;
}