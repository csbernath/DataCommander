namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DataReader : IDisposable
    {
        #region Private Fields

        private readonly IDbCommand command;
        private readonly IDataReader dataReader;
        private bool nextResultCalled = true;

        #endregion

        private DataReader(IDbCommand command, IDataReader dataReader)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
            Contract.Requires<ArgumentNullException>(dataReader != null);
#endif

            this.command = command;
            this.dataReader = dataReader;
        }

        internal static DataReader Create(
            IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition,
            CommandBehavior commandBehavior)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(commandDefinition != null);
#endif

            IDbCommand command = null;
            IDataReader dataReader = null;

            try
            {
                command = transactionScope.CreateCommand(commandDefinition);
                dataReader = command.ExecuteReader(commandBehavior);
                return new DataReader(command, dataReader);
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Dispose();
                }

                if (command != null)
                {
                    command.Dispose();
                }

                throw;
            }
        }

#region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="read"></param>
        /// <returns></returns>
        public IEnumerable<T> Read<T>(Func<IDataRecord, T> read)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(read != null);
#endif
            this.PrivateNextResult();

            while (this.dataReader.Read())
            {
                yield return read(this.dataReader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        public void Read(Action<IDataRecord> read)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(read != null);
#endif
            this.PrivateNextResult();

            while (this.dataReader.Read())
            {
                read(this.dataReader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        public void Read(Func<IDataRecord, bool> read)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(read != null);
#endif
            this.PrivateNextResult();

            while (this.dataReader.Read())
            {
                var succeeded = read(this.dataReader);
                if (!succeeded)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool NextResult()
        {
#if CONTRACTS_FULL
            Contract.Assert(this.dataReader != null);
            Contract.Assert(!this.nextResultCalled);
#endif

            var nextResult = this.dataReader.NextResult();
            this.nextResultCalled = true;

            return nextResult;
        }

#endregion

        void IDisposable.Dispose()
        {
            this.dataReader.Dispose();
            this.command.Dispose();
        }

        private void PrivateNextResult()
        {
            if (this.nextResultCalled)
            {
                this.nextResultCalled = false;
            }
            else
            {
                var nextResult = this.dataReader.NextResult();
                this.nextResultCalled = true;

                if (!nextResult)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}