using DataCommander.Api.Connection;
using System.Data.Common;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.OleDb;

internal sealed class Connection : ConnectionBase
{
    private readonly OleDbConnection _oledbConnection;

    public Connection(string connectionString)
    {
        _oledbConnection = new OleDbConnection(connectionString);
        _oledbConnection.InfoMessage += OnInfoMessage;
        SetConnection(_oledbConnection);
    }

    public override Task OpenAsync(CancellationToken cancellationToken) => _oledbConnection.OpenAsync(cancellationToken);

    void OnInfoMessage(object? sender, OleDbInfoMessageEventArgs e)
    {
        var text = e.Message;
        InvokeInfoMessage([InfoMessageFactory.Create(InfoMessageSeverity.Information, null, text)]);
    }

    public override string DataSource => _oledbConnection.DataSource;
    public override string ServerVersion => _oledbConnection.ServerVersion;
    public override string? ConnectionInformation { get; }
    public override DbCommand CreateCommand() => _oledbConnection.CreateCommand();
    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => Task.FromResult(0);
}