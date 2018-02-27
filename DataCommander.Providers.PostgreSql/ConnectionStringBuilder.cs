namespace DataCommander.Providers.PostgreSql
{
    using System.Linq;
    using Npgsql;
    using Providers;
    using Providers.Connection;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => npgsqlConnectionStringBuilder.ConnectionString;

            set => npgsqlConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            var supportedKeywords = new[]
            {
                "Integrated Security"
            };

            return supportedKeywords.Contains(keyword);
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value)
        {
            bool contains;

            switch (keyword)
            {
                case ConnectionStringKeyword.DataSource:
                    contains = npgsqlConnectionStringBuilder.TryGetValue("Host", out value);
                    break;

                case ConnectionStringKeyword.InitialCatalog:
                    contains = npgsqlConnectionStringBuilder.TryGetValue("Database", out value);
                    break;

                default:
                    contains = npgsqlConnectionStringBuilder.TryGetValue(keyword, out value);
                    break;
            }

            return contains;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            switch (keyword)
            {
                case ConnectionStringKeyword.DataSource:
                    npgsqlConnectionStringBuilder.Host = (string)value;
                    break;

                case ConnectionStringKeyword.InitialCatalog:
                    npgsqlConnectionStringBuilder.Database = (string)value;
                    break;

                default:
                    npgsqlConnectionStringBuilder[keyword] = value;
                    break;
            }
        }
    }
}