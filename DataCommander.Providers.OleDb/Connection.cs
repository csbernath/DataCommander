namespace DataCommander.Providers.OleDb
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    using DataCommander.Foundation;

    internal sealed class Connection : ConnectionBase
    {
        private readonly OleDbConnection oledbConnection;
        private string connectionName;

        public Connection(string connectionString)
        {
            this.oledbConnection = new OleDbConnection(connectionString);
            this.Connection = this.oledbConnection;

            this.oledbConnection.InfoMessage += new OleDbInfoMessageEventHandler(this.OnInfoMessage);
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
            this.oledbConnection.Open();
        }

        public override string Caption
        {
            get
            {
                return this.oledbConnection.ConnectionString;
            }
        }

        void OnInfoMessage( object sender, OleDbInfoMessageEventArgs e )
        {
            DateTime now = LocalTime.Default.Now;
            string text = e.Message;
            this.InvokeInfoMessage( new InfoMessage[] { new InfoMessage( now, InfoMessageSeverity.Information, text ) } );
        }

        public override string DataSource
        {
            get
            {
                return this.oledbConnection.DataSource;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return this.oledbConnection.ServerVersion;
            }
        }

        public override IDbCommand CreateCommand()
        {
            return this.oledbConnection.CreateCommand();
        }

        protected override void SetDatabase(string database)
        {
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