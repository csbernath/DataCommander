using System.Security;
using DataCommander.Api;
using DataCommander.Api.Connection;

namespace DataCommander.Application.Connection;

public sealed class ConnectionProperties(
    string connectionName,
    string providerIdentifier,
    IProvider? provider,
    string connectionString,
    byte[]? password)
{
    private string _connectionString = connectionString;

    public readonly string ConnectionName = connectionName;
    public readonly string ProviderIdentifier = providerIdentifier;
    public readonly IProvider? Provider = provider;
    public string ConnectionString => _connectionString;
    public readonly byte[]? Password = password;
    public ConnectionBase Connection;

    public SecureString? GetPasswordSecureString()
    {
        SecureString? secureString = null;
        if (Password != null)
        {
            var unprotectedPassword = ConnectionPropertiesRepository.UnprotectPassword(Password);
            secureString = new SecureString();
            foreach (var character in unprotectedPassword)
                secureString.AppendChar(character);
            secureString.MakeReadOnly();
        }

        return secureString;
    }
}