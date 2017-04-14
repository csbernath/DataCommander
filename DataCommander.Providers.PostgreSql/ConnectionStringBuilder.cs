namespace DataCommander.Providers.PostgreSql
{
    using System.Linq;
    using Npgsql;
    using DataCommander.Providers;

    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => this.npgsqlConnectionStringBuilder.ConnectionString;

            set => this.npgsqlConnectionStringBuilder.ConnectionString = value;
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
                    contains = this.npgsqlConnectionStringBuilder.TryGetValue("Host", out value);
                    break;

                case ConnectionStringKeyword.InitialCatalog:
                    contains = this.npgsqlConnectionStringBuilder.TryGetValue("Database", out value);
                    break;

                default:
                    contains = this.npgsqlConnectionStringBuilder.TryGetValue(keyword, out value);
                    break;
            }

            return contains;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            switch (keyword)
            {
                case ConnectionStringKeyword.DataSource:
                    this.npgsqlConnectionStringBuilder.Host = (string)value;
                    break;

                case ConnectionStringKeyword.InitialCatalog:
                    this.npgsqlConnectionStringBuilder.Database = (string)value;
                    break;

                default:
                    this.npgsqlConnectionStringBuilder[keyword] = value;
                    break;
            }
        }
    }
}