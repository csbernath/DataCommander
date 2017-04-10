namespace DataCommander.Providers.OracleClient
{
    using System;
    using System.Data;
    using System.Data.OracleClient;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Foundation;

    internal sealed class Connection : ConnectionBase
    {
        private string connectionName;

        public Connection(string connectionString)
        {
#pragma warning disable 618
            oracleConnection = new OracleConnection(connectionString);
#pragma warning restore 618
            this.Connection = oracleConnection;
            oracleConnection.InfoMessage += new OracleInfoMessageEventHandler(OnInfoMessage);
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return this.oracleConnection.OpenAsync(cancellationToken);
        }

        public override string Caption => null;

        private void OnInfoMessage(object sender, OracleInfoMessageEventArgs e)
        {
            var now = LocalTime.Default.Now;

            var sb = new StringBuilder();
            sb.Append(e.Message);
            sb.Append(Environment.NewLine);
            sb.Append("Code: ");
            sb.Append(e.Code);
            sb.Append(Environment.NewLine);
            sb.Append("Source: ");
            sb.Append(e.Source);

            this.InvokeInfoMessage(new InfoMessage[] {new InfoMessage(now, InfoMessageSeverity.Information, sb.ToString())});
        }

        public override string DataSource => oracleConnection.DataSource;

        public override string ServerVersion => oracleConnection.ServerVersion;

        public override int TransactionCount => 0;

        public override IDbCommand CreateCommand()
        {
            return oracleConnection.CreateCommand();
        }

        protected override void SetDatabase(string database)
        {
        }

#pragma warning disable 618
        private readonly OracleConnection oracleConnection;
#pragma warning restore 618

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
    }
}