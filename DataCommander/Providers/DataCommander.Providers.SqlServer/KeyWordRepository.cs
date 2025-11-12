using System;
using System.Collections.Generic;
using System.Linq;
using DataCommander.Api;
using Foundation.Configuration;

namespace DataCommander.Providers.SqlServer;

internal static class KeyWordRepository
{
    private static IReadOnlySet<string>? _keyWords;

    public static IReadOnlySet<string> Get()
    {
        if (_keyWords == null)
        {
            var path = ConfigurationNodeName.FromType(typeof(SqlServerProvider));
            var folder = Settings.SelectNode(path, true)!;
            var transactSqlKeyWordsArray = folder.Attributes["TSqlKeyWords"].GetValue<string[]>()!;
            Words.AssertOrder(transactSqlKeyWordsArray);
            _keyWords = transactSqlKeyWordsArray.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        return _keyWords;
    }
}