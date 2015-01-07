namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Data.SQLite;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Providers;

    internal sealed class Connection : ConnectionBase
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private SQLiteConnection sqliteConnection;
        private string connectionName;

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

        public override string ConnectionName
        {
            get
            {
                return this.connectionName;
            }

            set
            {
                this.connectionName = value;
            }
        }

        public override void Open()
        {
            sqliteConnection.Open();
        }

        public override System.Data.IDbCommand CreateCommand()
        {
            return sqliteConnection.CreateCommand();
        }

        public override string Caption
        {
            get
            {
                return sqliteConnection.DataSource;
            }
        }

        public override string DataSource
        {
            get
            {
                return sqliteConnection.DataSource;
            }
        }

        protected override void SetDatabase(string database)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string ServerVersion
        {
            get
            {
                return sqliteConnection.ServerVersion;
            }
        }

        public override int TransactionCount
        {
            get
            {
                // TODO
                return 0;
            }
        }
    }
}