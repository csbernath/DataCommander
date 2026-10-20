using DataCommander.Api.Connection;
using MongoDB.Driver;

namespace DataCommander.Providers.MongoDB;

public static class MongoConnectionStringFactory
{
    public static string Create(ConnectionStringAndCredential connectionStringAndCredential)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionStringAndCredential.ConnectionString);
        var credential = connectionStringAndCredential.Credential!;
        mongoUrlBuilder.Username = credential.UserId;
        mongoUrlBuilder.Password = PasswordFactory.Unprotect(credential.Password.Protected);
        var connectionString = mongoUrlBuilder.ToString();
        return connectionString;
    }
}