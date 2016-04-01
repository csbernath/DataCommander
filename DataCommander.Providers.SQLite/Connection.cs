namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class Connection : ConnectionBase
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly SQLiteConnection sqliteConnection;

        public Connection( string connectionString )
        {
            this.sqliteConnection = new SQLiteConnection( connectionString );
            // this.sqliteConnection.Flags = SQLiteConnectionFlags.LogAll;
            // this.sqliteConnection.Trace += this.sqliteConnection_Trace;
            this.Connection = this.sqliteConnection;
        }

        void SQLiteLog_Log( object sender, LogEventArgs e )
        {
        }

        private void sqliteConnection_Trace( object sender, TraceEventArgs e )
        {
            log.Write( LogLevel.Trace,  e.Statement );
        }

        public override string ConnectionName { get; set; }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return this.sqliteConnection.OpenAsync(cancellationToken);
        }

        public override IDbCommand CreateCommand()
        {
            return this.sqliteConnection.CreateCommand();
        }

        public override string Caption => this.sqliteConnection.DataSource;

        public override string DataSource => this.sqliteConnection.DataSource;

        protected override void SetDatabase(string database)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ServerVersion => this.sqliteConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}