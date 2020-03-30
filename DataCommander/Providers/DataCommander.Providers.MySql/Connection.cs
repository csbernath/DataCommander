using System;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Providers.Connection;
using DataCommander.Providers2.Connection;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql
{
    internal sealed class Connection : ConnectionBase
    {
        private readonly string _connectionString;
        private readonly MySqlConnection _mySqlConnection;
        private string _connectionName;

        public Connection(string connectionString)
        {
            _connectionString = connectionString;
            _mySqlConnection = new MySqlConnection(connectionString);
            Connection = _mySqlConnection;
        }

        public override Task OpenAsync(CancellationToken cancellationToken) => _mySqlConnection.OpenAsync(cancellationToken);
        public override System.Data.IDbCommand CreateCommand() => _mySqlConnection.CreateCommand();

        public override string ConnectionName
        {
            get => _connectionName;
            set => _connectionName = value;
        }

        public override string Caption => _connectionName;

        public override string DataSource
        {
            get
            {
                // TODO
                var csb = new MySqlConnectionStringBuilder(_connectionString);
                return csb.Database;
            }
        }

        protected override void SetDatabase(string database)
        {
            throw new NotImplementedException();
        }

        public override string ServerVersion => _mySqlConnection.ServerVersion;

        public override int TransactionCount => 0;
    }
}