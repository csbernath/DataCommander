namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal class TfsCommand : IDbCommand
    {
        public TfsCommand(TfsConnection connection)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires(connection != null);
#endif
            this.Connection = connection;
        }

        public TfsConnection Connection { get; private set; }

#region IDbCommand Members

        public void Cancel()
        {
            this.Cancelled = true;
        }

        internal bool Cancelled { get; private set; }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        IDbConnection IDbCommand.Connection
        {
            get => this.Connection.Connection;

            set
            {
                var tfsDbConnection = (TfsDbConnection)value;
                this.Connection = tfsDbConnection.Connection;
            }
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            throw new NotSupportedException();
        }

        int IDbCommand.ExecuteNonQuery()
        {
            throw new NotSupportedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            IDataReader dataReader;

            switch (this.CommandType)
            {
                case CommandType.StoredProcedure:
                    dataReader = this.ExecuteStoredProcedure(behavior);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return dataReader;
        }

        public IDataReader ExecuteReader()
        {
            return this.ExecuteReader(CommandBehavior.Default);
        }

        object IDbCommand.ExecuteScalar()
        {
            throw new NotSupportedException();
        }

        IDataParameterCollection IDbCommand.Parameters => this.Parameters;

        public TfsParameterCollection Parameters { get; } = new TfsParameterCollection();

        void IDbCommand.Prepare()
        {
            throw new NotSupportedException();
        }

        IDbTransaction IDbCommand.Transaction
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get => throw new NotImplementedException();

            set => throw new NotImplementedException();
        }

#endregion

#region IDisposable Members

        public void Dispose()
        {
        }

#endregion

        private IDataReader ExecuteStoredProcedure(CommandBehavior behavior)
        {
            IDataReader dataReader;
            TfsDataReaderFactory.DataReaderInfo info;
            var contains = TfsDataReaderFactory.Dictionary.TryGetValue(this.CommandText, out info);

            if (contains)
            {
                dataReader = info.CreateDataReader(this);
            }
            else
            {
                throw new NotSupportedException();
            }

            return dataReader;
        }
    }
}