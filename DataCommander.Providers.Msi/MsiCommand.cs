namespace DataCommander.Providers.Msi
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    internal sealed class MsiCommand : IDbCommand
    {
        #region Private Fields

        private MsiConnection connection;
        private CommandType commandType;
        private int commandTimeout;
        private string commandText;
        private MsiParameterCollection parameters = new MsiParameterCollection();

        #endregion

        public MsiCommand(MsiConnection connection)
        {
            Contract.Requires(connection != null);
            this.connection = connection;
        }

        public MsiConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        #region IDbCommand Members

        void IDbCommand.Cancel()
        {
            throw new NotImplementedException();
        }

        public string CommandText
        {
            get
            {
                return this.commandText;
            }

            set
            {
                this.commandText = value;
            }
        }

        int IDbCommand.CommandTimeout
        {
            get
            {
                return this.commandTimeout;
            }

            set
            {
                this.commandTimeout = value;
            }
        }

        CommandType IDbCommand.CommandType
        {
            get
            {
                return this.commandType;
            }

            set
            {
                this.commandType = value;
            }
        }

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

        IDataParameterCollection IDbCommand.Parameters
        {
            get
            {
                return this.parameters;
            }
        }

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