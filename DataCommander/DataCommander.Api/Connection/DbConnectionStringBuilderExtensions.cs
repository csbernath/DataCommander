using System.Data.Common;

namespace DataCommander.Api.Connection;

internal static class DbConnectionStringBuilderExtensions
{
    public static string? GetValue(this DbConnectionStringBuilder dbConnectionStringBuilder, string keyword)
    {
        var contains = dbConnectionStringBuilder.TryGetValue(keyword, out var valueObject);
        var value = contains ? (string?)valueObject : null;
        return value;
    }
}