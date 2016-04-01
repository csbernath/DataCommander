namespace DataCommander.Providers.OleDb
{
    using System;
    using System.Data;
    using System.Data.OleDb;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation;

    internal sealed class Connection : ConnectionBase
    {
        private readonly OleDbConnection oledbConnection;

        public Connection(string connectionString)
        {
            this.oledbConnection = new OleDbConnection(connectionString);
            this.Connection = this.oledbConnection;

            this.oledbConnection.InfoMessage += new OleDbInfoMessageEventHandler(this.OnInfoMessage);
        }

        public override string ConnectionName { get; set; }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return this.oledbConnection.OpenAsync(cancellationToken);
        }

        public override string Caption => this.oledbConnection.ConnectionString;

        void OnInfoMessage( object sender, OleDbInfoMessageEventArgs e )
        {
            DateTime now = LocalTime.Default.Now;
            string text = e.Message;
            this.InvokeInfoMessage( new InfoMessage[] { new InfoMessage( now, InfoMessageSeverity.Information, text ) } );
        }

        public override string DataSource => this.oledbConnection.DataSource;

        public override string ServerVersion => this.oledbConnection.ServerVersion;

        public override IDbCommand CreateCommand()
        {
            return this.oledbConnection.CreateCommand();
        }

        protected override void SetDatabase(string database)
        {
        }

        public override int TransactionCount => 0;
    }
}