using DataCommander.Api.Connection;

namespace DataCommander.Application.Connection;

public static class ConnectionInfoMapper
{
    extension(ConnectionInfo connectionInfo)
    {
        public ConnectionDto ToConnectionDto()
        {
            var credential = connectionInfo.ConnectionStringAndCredential.Credential;
            CredentialDto? credentialDto = null;
            if (credential != null)
            {
                var password = credential.Password.Protected;
                credentialDto = new CredentialDto(credential.UserId, password);
            }

            return new ConnectionDto(connectionInfo.ConnectionName!, connectionInfo.ProviderIdentifier,
                connectionInfo.ConnectionStringAndCredential.ConnectionString,
                credentialDto);
        }
    }

    extension(ConnectionDto connectionDto)
    {
        public ConnectionInfo ToConnectionProperties()
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
}