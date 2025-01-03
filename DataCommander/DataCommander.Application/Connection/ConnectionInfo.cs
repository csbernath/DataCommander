using DataCommander.Api.Connection;

namespace DataCommander.Application.Connection;

public sealed class ConnectionInfo(string connectionName, string providerIdentifier, ConnectionStringAndCredential connectionStringAndCredential)
{
    public readonly string ConnectionName = connectionName;
    public readonly string ProviderIdentifier = providerIdentifier;
    public readonly ConnectionStringAndCredential ConnectionStringAndCredential = connectionStringAndCredential;
}