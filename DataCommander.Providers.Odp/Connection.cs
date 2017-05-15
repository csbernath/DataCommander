namespace DataCommander.Providers.Odp
{
    using System.Data;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation;
    using Oracle.ManagedDataAccess.Client;
    using Providers.Connection;

    internal sealed class Connection : ConnectionBase
    {
		private readonly OracleConnectionStringBuilder oracleConnectionStringBuilder;
		private readonly OracleConnection oracleConnection;
		private string connectionName;

        public Connection(string connectionString)
        {
            this.oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connectionString);            
            this.oracleConnection = new OracleConnection(connectionString);
            this.Connection = oracleConnection;
            oracleConnection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            await this.oracleConnection.OpenAsync(cancellationToken);
            var enlist = bool.Parse(oracleConnectionStringBuilder.Enlist);
        }

        public override string Caption
        {
            get
            {
                var caption = $"{oracleConnectionStringBuilder.UserID}@{oracleConnectionStringBuilder.DataSource}";
                return caption;
            }
        }

        private void OnInfoMessage( object sender, OracleInfoMessageEventArgs e )
        {
            var now = LocalTime.Default.Now;

            var sb = new StringBuilder();
            sb.AppendLine( e.Message );
            sb.Append( "Source: " );
            sb.Append( e.Source );

            InvokeInfoMessage( new InfoMessage[] { new InfoMessage( now, InfoMessageSeverity.Information, sb.ToString() ) } );
        }

        public override string ConnectionName
        {
            get => this.connectionName;

            set => this.connectionName = value;
        }

        public override string DataSource => this.oracleConnection.DataSource;

        public override string ServerVersion => this.oracleConnection.ServerVersion;

        public override IDbCommand CreateCommand()
        {
            var command = this.oracleConnection.CreateCommand();
            command.InitialLONGFetchSize = 8 * 1024;
            command.FetchSize = 256 * 1024;
            return command;
        }

        protected override void SetDatabase(string database)
        {
        }

        public override int TransactionCount => 0;
    }
}