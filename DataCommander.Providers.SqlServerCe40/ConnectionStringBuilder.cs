namespace DataCommander.Providers.SqlServerCe40
{
    using System.Data.SqlServerCe;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SqlCeConnectionStringBuilder sqlCeConnectionStringBuilder = new SqlCeConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => sqlCeConnectionStringBuilder.ConnectionString;

            set => sqlCeConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return true;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            sqlCeConnectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return sqlCeConnectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}