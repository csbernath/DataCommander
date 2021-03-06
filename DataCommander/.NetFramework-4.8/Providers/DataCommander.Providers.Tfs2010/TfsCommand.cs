﻿namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    internal class TfsCommand : IDbCommand
    {
        private TfsConnection connection;
        private int commandTimeout;
        private string commandText;
        private CommandType commandType;
        private readonly TfsParameterCollection parameters = new TfsParameterCollection();
        private bool cancelled;

        public TfsCommand(TfsConnection connection)
        {
            Contract.Requires(connection != null);
            this.connection = connection;
        }

        public TfsConnection Connection => this.connection;

        #region IDbCommand Members

        public void Cancel()
        {
            this.cancelled = true;
        }

        internal bool Cancelled => this.cancelled;

        public string CommandText
        {
            get => this.commandText;

            set => this.commandText = value;
        }

        public int CommandTimeout
        {
            get => this.commandTimeout;

            set => this.commandTimeout = value;
        }

        public CommandType CommandType
        {
            get => this.commandType;

            set => this.commandType = value;
        }

        IDbConnection IDbCommand.Connection
        {
            get => this.connection.Connection;

            set
            {
                var tfsDbConnection = (TfsDbConnection)value;
                this.connection = tfsDbConnection.Connection;
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

            switch (this.commandType)
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

        IDataParameterCollection IDbCommand.Parameters => this.parameters;

        public TfsParameterCollection Parameters => this.parameters;

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
            var contains = TfsDataReaderFactory.Dictionary.TryGetValue(this.commandText, out info);

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