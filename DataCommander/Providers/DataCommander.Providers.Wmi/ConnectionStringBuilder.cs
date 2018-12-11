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

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return false;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            _dbConnectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return _dbConnectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}