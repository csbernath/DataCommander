using System;
using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class SafeDbCommand : IDbCommand
    {
        private readonly SafeDbConnection _connection;

        internal SafeDbCommand(
            SafeDbConnection connection,
            IDbCommand command )
        {
            _connection = connection;
            Command = command;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Command.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            Command.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter CreateParameter()
        {
            return Command.CreateParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            return _connection.ExecuteNonQuery(Command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            return _connection.ExecuteReader(Command, CommandBehavior.Default );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader( CommandBehavior behavior )
        {
            return _connection.ExecuteReader(Command, behavior );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            return _connection.ExecuteScalar(Command);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Prepare()
        {
            Command.Prepare();
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get => Command.CommandText;

            set => Command.CommandText = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout
        {
            get => Command.CommandTimeout;

            set => Command.CommandTimeout = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType
        {
            get => Command.CommandType;

            set => Command.CommandType = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection
        {
            get => _connection;

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameterCollection Parameters => Command.Parameters;

        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction Transaction
        {
            get => Command.Transaction;

            set => Command.Transaction = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateRowSource UpdatedRowSource
        {
            get => Command.UpdatedRowSource;

            set => Command.UpdatedRowSource = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommand Command { get; }
    }
}