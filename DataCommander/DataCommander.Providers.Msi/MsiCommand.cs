using System;
using System.Data;
using Foundation.Assertions;

namespace DataCommander.Providers.Msi
{
    internal sealed class MsiCommand : IDbCommand
    {
        #region Private Fields

        private readonly MsiParameterCollection _parameters = new MsiParameterCollection();

        #endregion

        public MsiCommand(MsiConnection connection)
        {
            Assert.IsNotNull(connection);
            Connection = connection;
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
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
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

        IDataParameterCollection IDbCommand.Parameters => _parameters;

        void IDbCommand.Prepare()
        {
            throw new NotImplementedException();
        }

        IDbTransaction IDbCommand.Transaction
        {
            get => null;

            set { }
        }

        UpdateRowSource IDbCommand.UpdatedRowSource
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}