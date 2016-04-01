namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public class AsyncDbCommand : IDbCommand
    {
        #region Private Fields

        private readonly AsyncDbConnection connection;
        private readonly IDbCommand command;

        #endregion

        internal AsyncDbCommand(AsyncDbConnection connection, IDbCommand command)
        {
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(command != null);

            this.connection = connection;
            this.command = command;
        }

        #region IDbCommand Members

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            // TODO:  Add AsyncDbCommand.Cancel implementation
        }

        /// <summary>
        /// 
        /// </summary>
        public void Prepare()
        {
            // TODO:  Add AsyncDbCommand.Prepare implementation
        }

        /// <summary>
        /// 
        /// </summary>
        public CommandType CommandType
        {
            get
            {
                Contract.Assert(this.command != null);

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
        /// <param name="behavior"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            // TODO:  Add AsyncDbCommand.ExecuteReader implementation
            return null;
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            // TODO:  Add AsyncDbCommand.System.Data.IDbCommand.ExecuteReader implementation
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            // TODO:  Add AsyncDbCommand.ExecuteScalar implementation
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            Contract.Assert(this.connection != null);

            return this.connection.ExecuteNonQuery(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                // TODO:  Add AsyncDbCommand.CommandTimeout getter implementation
                return 0;
            }

            set
            {
                // TODO:  Add AsyncDbCommand.CommandTimeout setter implementation
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDbDataParameter CreateParameter()
        {
            // TODO:  Add AsyncDbCommand.CreateParameter implementation
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                // TODO:  Add AsyncDbCommand.Connection getter implementation
                return null;
            }

            set
            {
                // TODO:  Add AsyncDbCommand.Connection setter implementation
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                // TODO:  Add AsyncDbCommand.UpdatedRowSource getter implementation
                return new UpdateRowSource();
            }

            set
            {
                // TODO:  Add AsyncDbCommand.UpdatedRowSource setter implementation
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandText
        {
            get
            {
                Contract.Assert(this.command != null);
                return this.command.CommandText;
            }

            set
            {
                Contract.Assert(this.command != null);
                this.command.CommandText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameterCollection Parameters
        {
            get
            {
                Contract.Assert(this.command != null);
                return this.command.Parameters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction Transaction
        {
            get
            {
                Contract.Assert(this.command != null);

                return this.command.Transaction;
            }

            set
            {
                Contract.Assert(this.command != null);

                this.command.Transaction = value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // TODO:  Add AsyncDbCommand.Dispose implementation
        }

        #endregion

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.connection != null);
            Contract.Invariant(this.command != null);
        }
    }
}