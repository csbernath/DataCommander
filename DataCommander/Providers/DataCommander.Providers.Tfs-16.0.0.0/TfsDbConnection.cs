using System;
using System.Data;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsDbConnection : IDbConnection
    {
        public TfsDbConnection(TfsConnection connection)
        {
            Connection = connection;
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
            get => $"Data Source={Connection.TfsTeamProjectCollection.Uri}";

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

        ConnectionState IDbConnection.State => Connection.ConnectionState;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // TODO
        }

        #endregion
    }
}
