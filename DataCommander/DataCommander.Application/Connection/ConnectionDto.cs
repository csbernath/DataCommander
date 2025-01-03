namespace DataCommander.Application.Connection;

public class ConnectionDto(string connectionName, string providerIdentifier, string connectionString, CredentialDto? credential)
{
    public readonly string ConnectionName = connectionName;
    public readonly string ProviderIdentifier = providerIdentifier;
    public readonly string ConnectionString = connectionString;
    public readonly CredentialDto? Credential = credential;
}