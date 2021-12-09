using DataCommander.Providers2;
using System;
using System.Data.OleDb;

namespace DataCommander.Providers.OleDb;

internal sealed class ConnectionStringBuilder : IDbConnectionStringBuilder
{
    private readonly OleDbConnectionStringBuilder oleDbConnectionStringBuilder = new();

    string IDbConnectionStringBuilder.ConnectionString
    {
        get => oleDbConnectionStringBuilder.ConnectionString;

        set => oleDbConnectionStringBuilder.ConnectionString = value;
    }

    bool IDbConnectionStringBuilder.IsKeywordSupported(string keyword) => true;
    void IDbConnectionStringBuilder.SetValue(string keyword, object value) => oleDbConnectionStringBuilder[keyword] = value;
    bool IDbConnectionStringBuilder.TryGetValue(string keyword, out object value) => oleDbConnectionStringBuilder.TryGetValue(keyword, out value);
    bool IDbConnectionStringBuilder.Remove(string keyword) => throw new NotImplementedException();
}