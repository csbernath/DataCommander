﻿using DataCommander.Providers2;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp
{
    internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
    {
        private readonly OracleConnectionStringBuilder _oracleConnectionStringBuilder = new OracleConnectionStringBuilder();

        string IDbConnectionStringBuilder.ConnectionString
        {
            get => _oracleConnectionStringBuilder.ConnectionString;
            set => _oracleConnectionStringBuilder.ConnectionString = value;
        }

        bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => false;
        bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _oracleConnectionStringBuilder.TryGetValue(keyword, out value);
        void IDbConnectionStringBuilder.SetValue(string keyword, object value) => _oracleConnectionStringBuilder[keyword] = value;
        bool IDbConnectionStringBuilder.Remove(string keyword) => _oracleConnectionStringBuilder.Remove(keyword);
    }
}