namespace DataCommander.Providers.Wmi
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class WmiProviderConnection : ConnectionBase
    {
        private readonly WmiConnection wmiConnection;
        private string connectionName;

        public WmiProviderConnection(string connectionString)
        {
            this.wmiConnection = new WmiConnection(connectionString);
            this.Connection = this.wmiConnection;
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

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(this.wmiConnection.Open);
        }

        public override IDbCommand CreateCommand()
        {
            return this.wmiConnection.CreateCommand();
        }

        public override string Caption => $"WMI@{this.wmiConnection.DataSource}";

        public override string DataSource => this.wmiConnection.DataSource;

        public override string ServerVersion => null;

        public override int TransactionCount => 0;

        protected override void SetDatabase(string database)
        {
        }
    }
}