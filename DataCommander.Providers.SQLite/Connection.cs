using System;
using System.Data;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Providers.Connection;
using Foundation.Log;

namespace DataCommander.Providers.SQLite
{
    internal sealed class Connection : ConnectionBase
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly SQLiteConnection _sqliteConnection;

        public Connection(string connectionString)
        {
            _sqliteConnection = new SQLiteConnection(connectionString);
            // this.sqliteConnection.Flags = SQLiteConnectionFlags.LogAll;
            // this.sqliteConnection.Trace += this.sqliteConnection_Trace;
            Connection = _sqliteConnection;
        }

        void SQLiteLog_Log(object sender, LogEventArgs e)
        {
        }

        private void sqliteConnection_Trace(object sender, TraceEventArgs e)
        {
            Log.Write(LogLevel.Trace, e.Statement);
        }

        public override string ConnectionName { get; set; }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return _sqliteConnection.OpenAsync(cancellationToken);
        }

        public override IDbCommand CreateCommand()
        {
            return _sqliteConnection.CreateCommand();
        }

        public override string Caption => _sqliteConnection.DataSource;

        public override string DataSource => _sqliteConnection.DataSource;

        protected override void SetDatabase(string database)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ServerVersion => _sqliteConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}