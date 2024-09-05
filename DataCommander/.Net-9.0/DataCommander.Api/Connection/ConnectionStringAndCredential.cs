namespace DataCommander.Api.Connection;

public sealed class ConnectionStringAndCredential(string connectionString, Credential? credential)
{
    public readonly string ConnectionString = connectionString;
    public readonly Credential? Credential = credential;
}