using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataCommander.Providers.MongoDB;

public class MongoConnection : ConnectionBase
{
    private readonly ConnectionStringAndCredential _connectionStringAndCredential;

    public MongoConnection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        _connectionStringAndCredential = connectionStringAndCredential;
    }

    public ConnectionStringAndCredential ConnectionStringAndCredential => _connectionStringAndCredential;

    public override Task OpenAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public override DbCommand CreateCommand() => new MongoCommand(this);

    public override string DataSource
    {
        get
        {
            var mongoConnectionStringBuilder = new MongoConnectionStringBuilder(_connectionStringAndCredential.ConnectionString);
            mongoConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.DataSource, out var dataSource);
            return (string)dataSource!;
        }
    }

    public override string ServerVersion
    {
        get
        {
            var connectionString = MongoConnectionStringFactory.Create(_connectionStringAndCredential);
            using var client = new MongoClient(connectionString);
            var adminDatabase = client.GetDatabase("admin");
            var command = new BsonDocument("buildInfo", 1);
            var result = adminDatabase.RunCommand<BsonDocument>(command);
            var serverVersion = result["version"].AsString;
            return serverVersion;
        }
    }

    public override string? ConnectionInformation => null;

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();
}