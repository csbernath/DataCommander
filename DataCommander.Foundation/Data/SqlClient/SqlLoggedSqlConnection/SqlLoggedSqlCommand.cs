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
            Contract.Requires(connection != null);
            Contract.Requires(command != null);
#endif

            this._connection = connection;
            this._command = command;
        }

        public string CommandText
        {
            get => this._command.CommandText;

            set => this._command.CommandText = value;
        }

        public int CommandTimeout
        {
            get => this._command.CommandTimeout;

            set => this._command.CommandTimeout = value;
        }

        public CommandType CommandType
        {
            get => this._command.CommandType;

            set => this._command.CommandType = value;
        }

        public IDbConnection Connection
        {
            get => this._connection;

            set => throw new NotImplementedException();
        }

        public IDataParameterCollection Parameters => this._command.Parameters;

        public IDbTransaction Transaction
        {
            get => this._command.Transaction;

            set => this._command.Transaction = value;
        }

        public UpdateRowSource UpdatedRowSource
        {
            get => this._command.UpdatedRowSource;

            set => this._command.UpdatedRowSource = value;
        }

        public void Dispose()
        {
            this._command.Dispose();
        }

        public void Cancel()
        {
            this._command.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return this._command.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            return this._connection.ExecuteNonQuery(this._command);
        }

        public IDataReader ExecuteReader()
        {
            var loggedSqlDataReader = new SqlLoggedSqlDataReader(this._connection, this._command);
            return loggedSqlDataReader.Execute();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            var loggedSqlDataReader = new SqlLoggedSqlDataReader(this._connection, this._command);
            return loggedSqlDataReader.Execute(behavior);
        }

        public object ExecuteScalar()
        {
            return this._connection.ExecuteScalar(this._command);
        }

        public void Prepare()
        {
            this._command.Prepare();
        }
    }
}