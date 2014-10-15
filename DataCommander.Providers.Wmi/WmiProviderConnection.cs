namespace DataCommander.Providers.Wmi
{
    using System.Data;

    internal sealed class WmiProviderConnection : ConnectionBase
    {
        private WmiConnection wmiConnection;
        private string connectionName;

        public WmiProviderConnection(string connectionString)
        {
            this.wmiConnection = new WmiConnection(connectionString);
            this.Connection = wmiConnection;
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
            wmiConnection.Open();
        }

        public override IDbCommand CreateCommand()
        {
            return this.wmiConnection.CreateCommand();
        }

        public override string Caption
        {
            get
            {
                return string.Format("WMI@{0}", wmiConnection.DataSource);
            }
        }

        public override string DataSource
        {
            get
            {
                return wmiConnection.DataSource;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return null;
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

        protected override void SetDatabase(string database)
        {
        }
    }
}