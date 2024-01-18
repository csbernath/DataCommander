using DataCommander.Api.Connection;

namespace DataCommander.Application.Connection;

public static class ConnectionInfoMapper
{
    public static ConnectionDto ToConnectionDto(this ConnectionInfo connectionInfo)
    {
        var credential = connectionInfo.ConnectionStringAndCredential.Credential;
        CredentialDto? credentialDto = null;
        if (credential != null)
        {
            var password = credential.Password.Protected;
            credentialDto = new CredentialDto(credential.UserId, password);
        }

        return new ConnectionDto(connectionInfo.ConnectionName, connectionInfo.ProviderIdentifier,
            connectionInfo.ConnectionStringAndCredential.ConnectionString,
            credentialDto);
    }

    public static ConnectionInfo ToConnectionProperties(this ConnectionDto connectionDto)
    {
        Credential? credential = null;
        if (connectionDto.Credential != null)
        {
            var password = PasswordFactory.CreateFromProtected(connectionDto.Credential.Password);
            credential = new Credential(connectionDto.Credential.UserId, password);
        }

        return new ConnectionInfo(connectionDto.ConnectionName, connectionDto.ProviderIdentifier,
            new ConnectionStringAndCredential(connectionDto.ConnectionString, credential));
    }
}