﻿using System;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Microsoft.Data.Sqlite;

namespace DataCommander.Providers.SQLite;

internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
{
    private readonly SqliteConnectionStringBuilder _sqLiteConnectionStringBuilder = [];

    string IDbConnectionStringBuilder.ConnectionString
    {
        get => _sqLiteConnectionStringBuilder.ConnectionString;
        set => _sqLiteConnectionStringBuilder.ConnectionString = value;
    }

    bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => true;
    void IDbConnectionStringBuilder.SetValue(string keyword, object? value) => _sqLiteConnectionStringBuilder[keyword] = value;

    bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object? value)
    {
        bool contains;
        switch (keyword)
        {
            case ConnectionStringKeyword.IntegratedSecurity:
                contains = _sqLiteConnectionStringBuilder.TryGetValue(ConnectionStringKeyword.IntegratedSecurity, out value);
                if (contains)
                    value = bool.Parse((string) value!);

                break;

            default:
                contains = _sqLiteConnectionStringBuilder.TryGetValue(keyword, out value);
                break;
        }

        return contains;
    }

    bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
}