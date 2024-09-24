using System;
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

    void SQLiteLog_Log(object sender, LogEventArgs e)
    {
    }

    private void sqliteConnection_Trace(object sender, TraceEventArgs e) => Log.Write(LogLevel.Trace, e.Statement);

    public override string ConnectionName { get; set; }

    public override Task OpenAsync(CancellationToken cancellationToken) => _sqliteConnection.OpenAsync(cancellationToken);

    public override DbCommand CreateCommand() => _sqliteConnection.CreateCommand();

    public override string Caption => _sqliteConnection.DataSource;

    public override string DataSource => _sqliteConnection.DataSource;

    protected static void SetDatabase(string database) => throw new Exception("The method or operation is not implemented.");

    public override string ServerVersion => _sqliteConnection.ServerVersion;

    public override string ConnectionInformation => null;

    public override Task<int> GetTransactionCountAsync(CancellationToken cancellationToken) => Task.FromResult(0);
}