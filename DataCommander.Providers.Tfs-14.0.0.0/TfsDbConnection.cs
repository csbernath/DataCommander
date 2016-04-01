namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal sealed class TfsDbConnection : IDbConnection
    {
        public TfsDbConnection(TfsConnection connection)
        {
            this.Connection = connection;
        }

        public TfsConnection Connection { get; }

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
                return $"Data Source={this.Connection.TfsTeamProjectCollection.Uri}";
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

        string IDbConnection.Database => null;

        void IDbConnection.Open()
        {
            throw new NotImplementedException();
        }

        ConnectionState IDbConnection.State => this.Connection.ConnectionState;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO
        }

        #endregion
    }
}
