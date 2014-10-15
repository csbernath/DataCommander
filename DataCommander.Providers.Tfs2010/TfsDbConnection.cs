namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal sealed class TfsDbConnection : IDbConnection
    {
        private TfsConnection connection;

        public TfsDbConnection(TfsConnection connection)
        {
            this.connection = connection;
        }

        public TfsConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

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
        }

        string IDbConnection.ConnectionString
        {
            get
            {
                return string.Format("Data Source={0}", this.connection.TfsTeamProjectCollection.Uri);
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        int IDbConnection.ConnectionTimeout
        {
            get { throw new NotImplementedException(); }
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            throw new NotImplementedException();
        }

        string IDbConnection.Database
        {
            get
            {
                return null;
            }
        }

        void IDbConnection.Open()
        {
            throw new NotImplementedException();
        }

        ConnectionState IDbConnection.State
        {
            get
            {
                return this.connection.ConnectionState;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO
        }

        #endregion
    }
}
