using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api.Connection;
using Foundation.Log;

namespace DataCommander.Providers.SQLite;

internal sealed class Connection : ConnectionBase
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly SQLiteConnection _sqliteConnection;

    public Connection(ConnectionStringAndCredential connectionStringAndCredential)
    {
        _sqliteConnection = new SQLiteConnection(connectionStringAndCredential.ConnectionString);
        // this.sqliteConnection.Flags = SQLiteConnectionFlags.LogAll;
        // this.sqliteConnection.Trace += this.sqliteConnection_Trace;
        Connection = _sqliteConnection;
    }

    public override Task OpenAsync(CancellationToken cancellationToken) => _sqliteConnection.OpenAsync(cancellationToken);

    public override DbCommand CreateCommand() => _sqliteConnection.CreateCommand();

    public override string DataSource => _sqliteConnection.DataSource;

    public override string ServerVersion => _sqliteConnection.ServerVersion;

    public override string? ConnectionInformation => null;

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => Task.FromResult(0);
}