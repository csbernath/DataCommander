namespace DataCommander.Providers.SqlServerCe40
{
    using System.Data.SqlServerCe;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SqlCeConnectionStringBuilder _sqlCeConnectionStringBuilder = new SqlCeConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _sqlCeConnectionStringBuilder.ConnectionString;
            set => _sqlCeConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => true;
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => _sqlCeConnectionStringBuilder[keyword] = value;
        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _sqlCeConnectionStringBuilder.TryGetValue(keyword, out value);
    }
}