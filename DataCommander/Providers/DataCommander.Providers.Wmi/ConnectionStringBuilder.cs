using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace DataCommander.Providers.Wmi
{
    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly DbConnectionStringBuilder _dbConnectionStringBuilder = new SqlConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _dbConnectionStringBuilder.ConnectionString;

            set => _dbConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => false;
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => _dbConnectionStringBuilder[keyword] = value;
        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _dbConnectionStringBuilder.TryGetValue(keyword, out value);
        bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
    }
}