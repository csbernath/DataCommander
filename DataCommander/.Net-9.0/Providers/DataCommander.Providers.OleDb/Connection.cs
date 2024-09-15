using DataCommander.Api.Connection;
using System.Data.Common;
using System.Data.OleDb;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.OleDb;

internal sealed class Connection : ConnectionBase
{
    private readonly OleDbConnection oledbConnection;

    public Connection(string connectionString)
    {
        oledbConnection = new OleDbConnection(connectionString);
        Connection = oledbConnection;
        oledbConnection.InfoMessage += OnInfoMessage;
    }

    public override string ConnectionName { get; set; }
    public override Task OpenAsync(CancellationToken cancellationToken) => oledbConnection.OpenAsync(cancellationToken);
    public override string Caption => oledbConnection.ConnectionString;

    void OnInfoMessage(object sender, OleDbInfoMessageEventArgs e)
    {
        var text = e.Message;
        InvokeInfoMessage([InfoMessageFactory.Create(InfoMessageSeverity.Information, null, text)]);
    }

    public override string DataSource => oledbConnection.DataSource;
    public override string ServerVersion => oledbConnection.ServerVersion;
    public override string ConnectionInformation { get; }
    public override DbCommand CreateCommand() => oledbConnection.CreateCommand();
    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => Task.FromResult(0);
}