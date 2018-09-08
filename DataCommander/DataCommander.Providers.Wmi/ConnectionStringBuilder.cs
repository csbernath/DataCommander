namespace DataCommander.Providers.Wmi
{
    using System.Data.Common;
    using System.Data.SqlClient;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly DbConnectionStringBuilder dbConnectionStringBuilder = new SqlConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => dbConnectionStringBuilder.ConnectionString;

            set => dbConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return false;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            dbConnectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return dbConnectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}