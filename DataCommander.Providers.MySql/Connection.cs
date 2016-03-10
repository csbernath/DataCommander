namespace DataCommander.Providers.MySql
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DataCommander.Providers;
    using global::MySql.Data.MySqlClient;

    internal sealed class Connection : ConnectionBase
    {
        private readonly string connectionString;
        private readonly MySqlConnection mySqlConnection;
        private string connectionName;

        public Connection(string connectionString)
        {
            this.connectionString = connectionString;
            this.mySqlConnection = new MySqlConnection(connectionString);
            this.Connection = this.mySqlConnection;
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return this.mySqlConnection.OpenAsync(cancellationToken);
        }

        public override System.Data.IDbCommand CreateCommand()
        {
            return this.mySqlConnection.CreateCommand();
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

        public override string Caption => this.connectionName;

        public override string DataSource
        {
            get
            {
                // TODO
                var csb = new MySqlConnectionStringBuilder(this.connectionString);
                return csb.Database;
            }
        }

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion => this.mySqlConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}
