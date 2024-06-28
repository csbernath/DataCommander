using System;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Linq;
using Npgsql;

namespace DataCommander.Providers.PostgreSql
{
    internal sealed class PostgreSqlConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly NpgsqlConnectionStringBuilder _npgsqlConnectionStringBuilder = new();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _npgsqlConnectionStringBuilder.ConnectionString;

            set => _npgsqlConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
            var supportedKeywords = new[]
            {
                ConnectionStringKeyword.Host
            };
            return supportedKeywords.Contains(keyword);
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _npgsqlConnectionStringBuilder.TryGetValue(keyword, out value);

        void IDbConnectionStringBuilder.SetValue(string keyword, object? value) => _npgsqlConnectionStringBuilder[keyword] = value;

        bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
    }
}