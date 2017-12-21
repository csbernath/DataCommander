using System;
using System.Data;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection
{
    internal sealed class SqlLoggedSqlCommand : IDbCommand
    {
        private readonly SqlLoggedSqlConnection _connection;
        private readonly IDbCommand _command;

        public SqlLoggedSqlCommand(
            SqlLoggedSqlConnection connection,
            IDbCommand command)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(connection != null);
            FoundationContract.Requires(command != null);
#endif

            _connection = connection;
            _command = command;
        }

        public string CommandText
        {
            get => _command.CommandText;

            set => _command.CommandText = value;
        }

        public int CommandTimeout
        {
            get => _command.CommandTimeout;

            set => _command.CommandTimeout = value;
        }

        public CommandType CommandType
        {
            get => _command.CommandType;

            set => _command.CommandType = value;
        }

        public IDbConnection Connection
        {
            get => _connection;

            set => throw new NotImplementedException();
        }

        public IDataParameterCollection Parameters => _command.Parameters;

        public IDbTransaction Transaction
        {
            get => _command.Transaction;

            set => _command.Transaction = value;
        }

        public UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;

            set => _command.UpdatedRowSource = value;
        }

        public void Dispose()
        {
            _command.Dispose();
        }

        public void Cancel()
        {
            _command.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return _command.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            return _connection.ExecuteNonQuery(_command);
        }

        public IDataReader ExecuteReader()
        {
            var loggedSqlDataReader = new SqlLoggedSqlDataReader(_connection, _command);
            return loggedSqlDataReader.Execute();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            var loggedSqlDataReader = new SqlLoggedSqlDataReader(_connection, _command);
            return loggedSqlDataReader.Execute(behavior);
        }

        public object ExecuteScalar()
        {
            return _connection.ExecuteScalar(_command);
        }

        public void Prepare()
        {
            _command.Prepare();
        }
    }
}