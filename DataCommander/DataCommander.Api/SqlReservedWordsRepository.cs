using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Configuration;

namespace DataCommander.Api;

public static class SqlReservedWordsRepository
{
    private static Lazy<HashSet<string>> SqlReservedWords = new(PrivateGet);

    public static IReadOnlySet<string> Get() => SqlReservedWords.Value;

    private static HashSet<string> PrivateGet()
    {
        var sqlReservedWordsArray = Settings.CurrentType.Attributes["SqlReservedWords"].GetValue<string[]>()!;
        Words.AssertOrder(sqlReservedWordsArray);
        var sqlReservedWords = sqlReservedWordsArray.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return sqlReservedWords;
    }
}