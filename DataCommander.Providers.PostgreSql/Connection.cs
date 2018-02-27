namespace DataCommander.Providers.PostgreSql
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Npgsql;
    using Providers.Connection;

    internal sealed class Connection : ConnectionBase
    {
        #region Private Fields

        private readonly string connectionString;
        private readonly NpgsqlConnection npgsqlConnection;

        #endregion

        public Connection(string connectionString)
        {
            this.connectionString = connectionString;
            npgsqlConnection = new NpgsqlConnection(connectionString);
            Connection = npgsqlConnection;
        }

        public override Task OpenAsync(CancellationToken cancellationToken)
        {
            return npgsqlConnection.OpenAsync(cancellationToken);
        }

        public override IDbCommand CreateCommand()
        {
            return npgsqlConnection.CreateCommand();
        }

        public override string ConnectionName { get; set; }

        public override string Caption => npgsqlConnection.Database;

        public override string DataSource => npgsqlConnection.DataSource;

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion => npgsqlConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}