namespace DataCommander.Providers.MySql
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::MySql.Data.MySqlClient;
    using Providers.Connection;

    internal sealed class Connection : ConnectionBase
    {
        private readonly string connectionString;
        private readonly MySqlConnection mySqlConnection;
        private string connectionName;

        public Connection(string connectionString)
        {
            this.connectionString = connectionString;
            mySqlConnection = new MySqlConnection(connectionString);
            Connection = mySqlConnection;
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return mySqlConnection.OpenAsync(cancellationToken);
        }

        public override System.Data.IDbCommand CreateCommand()
        {
            return mySqlConnection.CreateCommand();
        }

        public override string ConnectionName
        {
            get => connectionName;

            set => connectionName = value;
        }

        public override string Caption => connectionName;

        public override string DataSource
        {
            get
            {
                // TODO
                var csb = new MySqlConnectionStringBuilder(connectionString);
                return csb.Database;
            }
        }

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion => mySqlConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}
