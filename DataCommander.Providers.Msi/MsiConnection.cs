using Foundation.Assertions;

namespace DataCommander.Providers.Msi
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Microsoft.Deployment.WindowsInstaller;

    internal sealed class MsiConnection : IDbConnection
    {
        #region Private Fields

        private readonly string _connectionString;
        private ConnectionState _state;

        #endregion

        public MsiConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Open()
        {
            var sb = new DbConnectionStringBuilder();
            sb.ConnectionString = _connectionString;

            Assert.IsTrue(sb.ContainsKey("Data Source"));

            var dataSourceObject = sb["Data Source"];

            Assert.IsTrue(dataSourceObject is string);

            var path = (string) dataSourceObject;
            Database = new Database(path, DatabaseOpenMode.ReadOnly);
            _state = ConnectionState.Open;
        }

        public MsiCommand CreateCommand()
        {
            return new MsiCommand(this);
        }

        internal Database Database { get; private set; }

        #region IDbConnection Members

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            throw new NotImplementedException();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        void IDbConnection.Close()
        {
            Database.Close();
        }

        string IDbConnection.ConnectionString
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        int IDbConnection.ConnectionTimeout => throw new NotImplementedException();

        IDbCommand IDbConnection.CreateCommand()
        {
            throw new NotImplementedException();
        }

        string IDbConnection.Database => Database.FilePath;

        void IDbConnection.Open()
        {
            throw new NotImplementedException();
        }

        ConnectionState IDbConnection.State => _state;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Database.Dispose();
        }

        #endregion
    }
}