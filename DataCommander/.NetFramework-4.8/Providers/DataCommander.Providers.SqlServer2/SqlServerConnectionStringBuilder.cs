﻿using DataCommander.Providers2;
using Microsoft.Data.SqlClient;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer2
{
    internal sealed class SqlServerConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder = new SqlConnectionStringBuilder();

        #region IDbConnectionStringBuilder Members

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _sqlConnectionStringBuilder.ConnectionString;
            set => _sqlConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
        {
        var supportedKeywords = new[]
        {
            "Integrated Security",
            "TrustServerCertificate"
        };
        return supportedKeywords.Contains(keyword);
        }

        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _sqlConnectionStringBuilder.TryGetValue(keyword, out value);
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => _sqlConnectionStringBuilder[keyword] = value;
        bool IDbConnectionStringBuilder.Remove(string keyword) => _sqlConnectionStringBuilder.Remove(keyword);

        #endregion
    }
}