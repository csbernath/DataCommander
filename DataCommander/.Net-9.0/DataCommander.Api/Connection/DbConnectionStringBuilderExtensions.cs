using System.Data.Common;

namespace DataCommander.Api.Connection;

internal static class DbConnectionStringBuilderExtensions
{
    public static string? GetValue(this DbConnectionStringBuilder dbConnectionStringBuilder, string keyword)
    {
        bool contains = dbConnectionStringBuilder.TryGetValue(keyword, out object? valueObject);
        string? value = contains ? (string?)valueObject : null;
        return value;
    }
}