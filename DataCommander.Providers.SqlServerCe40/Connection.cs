namespace DataCommander.Providers.SqlServerCe40
{
    using System;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Threading;
    using System.Threading.Tasks;
    using Providers.Connection;

    internal sealed class Connection : ConnectionBase
    {
        private readonly SqlCeConnection sqlCeConnection;

        public Connection(string connectionString)
        {
            sqlCeConnection = new SqlCeConnection(connectionString);
            Connection = sqlCeConnection;
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return sqlCeConnection.OpenAsync(cancellationToken);
        }

        public override IDbCommand CreateCommand()
        {
            return sqlCeConnection.CreateCommand();
        }

        public override string ConnectionName { get; set; }

        public override string Caption => sqlCeConnection.DataSource;

        public override string DataSource => sqlCeConnection.DataSource;

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion => sqlCeConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}