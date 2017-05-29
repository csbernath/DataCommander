using System.Data;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncDbCommand : IDbCommand
    {
        #region Private Fields

        private readonly AsyncDbConnection _connection;
        private readonly IDbCommand _command;

        #endregion

        internal AsyncDbCommand(AsyncDbConnection connection, IDbCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(connection != null);
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            this._connection = connection;
            this._command = command;
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
#if CONTRACTS_FULL
                Contract.Assert(this.command != null);
#endif

                return this._command.CommandType;
            }

            set
            {
                this._command.CommandType = value;
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
#if CONTRACTS_FULL
            Contract.Assert(this.connection != null);
#endif

            return this._connection.ExecuteNonQuery(this);
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
#if CONTRACTS_FULL
                Contract.Assert(this.command != null);
#endif
                return this._command.CommandText;
            }

            set
            {
#if CONTRACTS_FULL
                Contract.Assert(this.command != null);
#endif
                this._command.CommandText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameterCollection Parameters
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Assert(this.command != null);
#endif
                return this._command.Parameters;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbTransaction Transaction
        {
            get
            {
#if CONTRACTS_FULL
                Contract.Assert(this.command != null);
#endif

                return this._command.Transaction;
            }

            set
            {
#if CONTRACTS_FULL
                Contract.Assert(this.command != null);
#endif

                this._command.Transaction = value;
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

        //[ContractInvariantMethod]
        private void ObjectInvariant()
        {
#if CONTRACTS_FULL
            Contract.Invariant(this.connection != null);
            Contract.Invariant(this.command != null);
#endif
        }
    }
}