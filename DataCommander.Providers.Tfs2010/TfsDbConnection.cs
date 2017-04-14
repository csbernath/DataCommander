namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;

    internal sealed class TfsDbConnection : IDbConnection
    {
        private readonly TfsConnection connection;

        public TfsDbConnection(TfsConnection connection)
        {
            this.connection = connection;
        }

        public TfsConnection Connection => this.connection;

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
            get => $"Data Source={this.connection.TfsTeamProjectCollection.Uri}";

            set => throw new NotImplementedException();
        }

        int IDbConnection.ConnectionTimeout => throw new NotImplementedException();

        IDbCommand IDbConnection.CreateCommand()
        {
            throw new NotImplementedException();
        }

        string IDbConnection.Database => null;

        void IDbConnection.Open()
        {
            throw new NotImplementedException();
        }

        ConnectionState IDbConnection.State => this.connection.ConnectionState;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO
        }

        #endregion
    }
}
