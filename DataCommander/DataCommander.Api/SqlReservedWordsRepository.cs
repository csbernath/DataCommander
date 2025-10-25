using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Configuration;

namespace DataCommander.Api;

public static class SqlReservedWordsRepository
{
    public static IReadOnlySet<string> Get()
    {
        var sqlReservedWordsArray = Settings.CurrentType.Attributes["SqlReservedWords"].GetValue<string[]>()!;
        Words.AssertOrder(sqlReservedWordsArray);
        var sqlReservedWords = sqlReservedWordsArray.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return sqlReservedWords;
    }
}