namespace DataCommander.Providers.SqlServerCe40
{
    using System.Data.SqlServerCe;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SqlCeConnectionStringBuilder sqlCeConnectionStringBuilder = new SqlCeConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => this.sqlCeConnectionStringBuilder.ConnectionString;

            set => this.sqlCeConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return true;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            this.sqlCeConnectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return this.sqlCeConnectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}