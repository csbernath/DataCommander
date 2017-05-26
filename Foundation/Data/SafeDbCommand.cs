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
            this._connection = connection;
            this.Command = command;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Command.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            this.Command.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter CreateParameter()
        {
            return this.Command.CreateParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            return this._connection.ExecuteNonQuery( this.Command );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            return this._connection.ExecuteReader( this.Command, CommandBehavior.Default );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader( CommandBehavior behavior )
        {
            return this._connection.ExecuteReader( this.Command, behavior );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            return this._connection.ExecuteScalar( this.Command );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Prepare()
        {
            this.Command.Prepare();
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get => this.Command.CommandText;

            set => this.Command.CommandText = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout
        {
            get => this.Command.CommandTimeout;

            set => this.Command.CommandTimeout = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType
        {
            get => this.Command.CommandType;

            set => this.Command.CommandType = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection
        {
            get => this._connection;

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameterCollection Parameters => this.Command.Parameters;

        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction Transaction
        {
            get => this.Command.Transaction;

            set => this.Command.Transaction = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateRowSource UpdatedRowSource
        {
            get => this.Command.UpdatedRowSource;

            set => this.Command.UpdatedRowSource = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommand Command { get; }
    }
}