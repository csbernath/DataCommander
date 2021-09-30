using DataCommander.Providers2;
using Foundation.Linq;
using System.Data.SqlClient;

namespace DataCommander.Providers.SqlServer
{
    internal sealed class SqlServerConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder = new();

        #region IDbConnectionStringBuilder Members

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _sqlConnectionStringBuilder.ConnectionString;
            set => _sqlConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            var suppoertedKeywords = new[] { "Integrated Security" };
            return suppoertedKeywords.Contains(keyword);
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _sqlConnectionStringBuilder.TryGetValue(keyword, out value);
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => _sqlConnectionStringBuilder[keyword] = value;
        bool IDbConnectionStringBuilder.Remove(string keyword) => _sqlConnectionStringBuilder.Remove(keyword);

        #endregion
    }
}