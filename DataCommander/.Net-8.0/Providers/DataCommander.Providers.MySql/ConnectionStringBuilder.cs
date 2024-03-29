﻿using DataCommander.Api;
using MySql.Data.MySqlClient;
using System;

namespace DataCommander.Providers.MySql;

internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
{
    private readonly MySqlConnectionStringBuilder _mySqlConnectionStringBuilder = new();

    string IDbConnectionStringBuilder.ConnectionString
    {
        get => _mySqlConnectionStringBuilder.ConnectionString;
        set => _mySqlConnectionStringBuilder.ConnectionString = value;
    }

    bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => throw new NotImplementedException();
    bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
    void IDbConnectionStringBuilder.SetValue(string keyword, object? value) => throw new NotImplementedException();
    bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => _mySqlConnectionStringBuilder.TryGetValue(keyword, out value);
}