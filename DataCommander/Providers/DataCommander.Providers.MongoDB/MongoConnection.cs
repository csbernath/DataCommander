using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;

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

    public override string DataSource => null;

    public override string ServerVersion => null;

    public override string? ConnectionInformation => null;

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => throw new System.NotImplementedException();
}