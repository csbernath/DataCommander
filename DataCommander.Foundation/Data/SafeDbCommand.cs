namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public class SafeDbCommand : IDbCommand
    {
        private readonly SafeDbConnection connection;
        private readonly IDbCommand command;

        internal SafeDbCommand(
            SafeDbConnection connection,
            IDbCommand command )
        {
            this.connection = connection;
            this.command = command;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.command.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            this.command.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter CreateParameter()
        {
            return this.command.CreateParameter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            return this.connection.ExecuteNonQuery( this.command );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            return this.connection.ExecuteReader( this.command, CommandBehavior.Default );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader( CommandBehavior behavior )
        {
            return this.connection.ExecuteReader( this.command, behavior );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            return this.connection.ExecuteScalar( this.command );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Prepare()
        {
            this.command.Prepare();
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.command.CommandText;
            }

            set
            {
                this.command.CommandText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                return this.command.CommandTimeout;
            }

            set
            {
                this.command.CommandTimeout = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                return this.command.CommandType;
            }

            set
            {
                this.command.CommandType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                return this.connection;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameterCollection Parameters => this.command.Parameters;

        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction Transaction
        {
            get
            {
                return this.command.Transaction;
            }

            set
            {
                this.command.Transaction = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this.command.UpdatedRowSource;
            }

            set
            {
                this.command.UpdatedRowSource = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommand Command => this.command;
    }
}