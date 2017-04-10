namespace DataCommander.Providers.Msi
{
    using System;
    using System.Data;

    internal sealed class MsiCommand : IDbCommand
    {
        #region Private Fields

        private MsiParameterCollection parameters = new MsiParameterCollection();

        #endregion

        public MsiCommand(MsiConnection connection)
        {
#if CONTRACTS_FULL
            Contract.Requires(connection != null);
#endif
            this.Connection = connection;
        }

        public MsiConnection Connection { get; }

#region IDbCommand Members

        void IDbCommand.Cancel()
        {
            throw new NotImplementedException();
        }

        public string CommandText { get; set; }

        int IDbCommand.CommandTimeout { get; set; }

        CommandType IDbCommand.CommandType { get; set; }

        IDbConnection IDbCommand.Connection
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            throw new NotImplementedException();
        }

        int IDbCommand.ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return new MsiDataReader(this, behavior);
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return new MsiDataReader(this, CommandBehavior.Default);
        }

        object IDbCommand.ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        IDataParameterCollection IDbCommand.Parameters => this.parameters;

        void IDbCommand.Prepare()
        {
            throw new NotImplementedException();
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

        UpdateRowSource IDbCommand.UpdatedRowSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
        }

#endregion
    }
}