using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal class TfsCommand : IDbCommand
    {
        public TfsCommand(TfsConnection connection)
        {
            FoundationContract.Requires<ArgumentException>(connection != null);

            Connection = connection;
        }

        public TfsConnection Connection { get; private set; }

#region IDbCommand Members

        public void Cancel()
        {
            Cancelled = true;
        }

        internal bool Cancelled { get; private set; }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        IDbConnection IDbCommand.Connection
        {
            get => Connection.Connection;

            set
            {
                var tfsDbConnection = (TfsDbConnection)value;
                Connection = tfsDbConnection.Connection;
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

            switch (CommandType)
            {
                case CommandType.StoredProcedure:
                    dataReader = ExecuteStoredProcedure(behavior);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return dataReader;
        }

        public IDataReader ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }

        object IDbCommand.ExecuteScalar()
        {
            throw new NotSupportedException();
        }

        IDataParameterCollection IDbCommand.Parameters => Parameters;

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
            var contains = TfsDataReaderFactory.Dictionary.TryGetValue(CommandText, out info);

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