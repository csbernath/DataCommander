namespace DataCommander.Providers.Tfs
{
    using System.Data.Common;

    internal sealed class TfsConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => connectionStringBuilder.ConnectionString;
            set => connectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            return false;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            connectionStringBuilder[keyword] = value;
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return connectionStringBuilder.TryGetValue(keyword, out value);
        }
    }
}