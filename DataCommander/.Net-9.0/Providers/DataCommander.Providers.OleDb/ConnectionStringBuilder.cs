using DataCommander.Api;
using System;
using System.Data.OleDb;
using DataCommander.Api.Connection;

namespace DataCommander.Providers.OleDb;

internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
{
    private readonly OleDbConnectionStringBuilder _oleDbConnectionStringBuilder = [];

    string IDbConnectionStringBuilder.ConnectionString
    {
        get => _oleDbConnectionStringBuilder.ConnectionString;

        set => _oleDbConnectionStringBuilder.ConnectionString = value;
    }

    bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword)
    {
        bool isKeywordSupported;
        switch (keyword)
        {
            case ConnectionStringKeyword.DataSource:
                isKeywordSupported = true;
                break;
            default:
                isKeywordSupported = false;
                break;
        }

        return isKeywordSupported;
    }

    void IDbConnectionStringBuilder.SetValue(string keyword, object? value) => _oleDbConnectionStringBuilder[keyword] = value;
    bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object? value) => _oleDbConnectionStringBuilder.TryGetValue(keyword, out value);
    bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
}