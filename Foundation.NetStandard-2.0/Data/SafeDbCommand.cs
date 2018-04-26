using System;
using System.Data;

namespace Foundation.Data
{
    public class SafeDbCommand : IDbCommand
    {
        private readonly SafeDbConnection _connection;

        internal SafeDbCommand(
            SafeDbConnection connection,
            IDbCommand command)
        {
            _connection = connection;
            Command = command;
        }

        public void Dispose() => Command.Dispose();
        public void Cancel() => Command.Cancel();
        public IDbDataParameter CreateParameter() => Command.CreateParameter();
        public int ExecuteNonQuery() => _connection.ExecuteNonQuery(Command);
        public IDataReader ExecuteReader() => _connection.ExecuteReader(Command, CommandBehavior.Default);
        public IDataReader ExecuteReader(CommandBehavior behavior) => _connection.ExecuteReader(Command, behavior);
        public object ExecuteScalar() => _connection.ExecuteScalar(Command);
        public void Prepare() => Command.Prepare();

        public string CommandText
        {
            get => Command.CommandText;
            set => Command.CommandText = value;
        }

        public int CommandTimeout
        {
            get => Command.CommandTimeout;
            set => Command.CommandTimeout = value;
        }

        public CommandType CommandType
        {
            get => Command.CommandType;
            set => Command.CommandType = value;
        }

        public IDbConnection Connection
        {
            get => _connection;
            set => throw new NotImplementedException();
        }

        public IDataParameterCollection Parameters => Command.Parameters;

        public IDbTransaction Transaction
        {
            get => Command.Transaction;
            set => Command.Transaction = value;
        }

        public UpdateRowSource UpdatedRowSource
        {
            get => Command.UpdatedRowSource;
            set => Command.UpdatedRowSource = value;
        }

        public IDbCommand Command { get; }
    }
}