namespace DataCommander.Providers.SqlServer2005
{
    using System.Data.SqlClient;
    using Foundation.Linq;

    internal sealed class SqlServerConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();

        #region IDbConnectionStringBuilder Members

        string IDbConnectionStringBuilder.ConnectionString
        {
            get
            {
                return this.sqlConnectionStringBuilder.ConnectionString;
            }

            set
            {
                this.sqlConnectionStringBuilder.ConnectionString = value;
            }
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            var suppoertedKeywords = new[]
            {
                "Integrated Security"
            };

            return suppoertedKeywords.Contains(keyword);
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            return this.sqlConnectionStringBuilder.TryGetValue(keyword, out value);
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            this.sqlConnectionStringBuilder[keyword] = value;
        }

        #endregion
    }
}