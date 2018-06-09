using System.Data.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly DbConnectionStringBuilder _connectionStringBuilder = new DbConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _connectionStringBuilder.ConnectionString;
            set => _connectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => false;
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => _connectionStringBuilder[keyword] = value;
        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _connectionStringBuilder.TryGetValue(keyword, out value);
    }
}