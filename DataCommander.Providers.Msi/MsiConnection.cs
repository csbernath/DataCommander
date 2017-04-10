namespace DataCommander.Providers.Msi
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Microsoft.Deployment.WindowsInstaller;

    internal sealed class MsiConnection : IDbConnection
    {
        #region Private Fields

        private readonly string connectionString;
        private ConnectionState state;

        #endregion

        public MsiConnection(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Open()
        {
            var sb = new DbConnectionStringBuilder();
            sb.ConnectionString = this.connectionString;
#if CONTRACTS_FULL
            Contract.Assert(sb.ContainsKey("Data Source"));
#endif
            var dataSourceObject = sb["Data Source"];
#if CONTRACTS_FULL
            Contract.Assert(dataSourceObject is string);
#endif
            var path = (string)dataSourceObject;
            this.Database = new Database(path, DatabaseOpenMode.ReadOnly);
            this.state = ConnectionState.Open;
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
            this.Database.Close();
        }

        string IDbConnection.ConnectionString
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

        int IDbConnection.ConnectionTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            throw new NotImplementedException();
        }

        string IDbConnection.Database => this.Database.FilePath;

        void IDbConnection.Open()
        {
            throw new NotImplementedException();
        }

        ConnectionState IDbConnection.State => this.state;

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            Database.Dispose();
        }

#endregion
    }
}