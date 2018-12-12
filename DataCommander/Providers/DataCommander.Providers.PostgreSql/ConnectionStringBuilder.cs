using System;
using System.Linq;
using DataCommander.Providers.Connection;
using Npgsql;

namespace DataCommander.Providers.PostgreSql
{
    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly NpgsqlConnectionStringBuilder _npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _npgsqlConnectionStringBuilder.ConnectionString;

            set => _npgsqlConnectionStringBuilder.ConnectionString = value;
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
                    contains = _npgsqlConnectionStringBuilder.TryGetValue("Host", out value);
                    break;

                case ConnectionStringKeyword.InitialCatalog:
                    contains = _npgsqlConnectionStringBuilder.TryGetValue("Database", out value);
                    break;

                default:
                    contains = _npgsqlConnectionStringBuilder.TryGetValue(keyword, out value);
                    break;
            }

            return contains;
        }

        void IDbConnectionStringBuilder.SetValue(string keyword, object value)
        {
            switch (keyword)
            {
                case ConnectionStringKeyword.DataSource:
                    _npgsqlConnectionStringBuilder.Host = (string) value;
                    break;

                case ConnectionStringKeyword.InitialCatalog:
                    _npgsqlConnectionStringBuilder.Database = (string) value;
                    break;

                default:
                    _npgsqlConnectionStringBuilder[keyword] = value;
                    break;
            }
        }

        bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
    }
}