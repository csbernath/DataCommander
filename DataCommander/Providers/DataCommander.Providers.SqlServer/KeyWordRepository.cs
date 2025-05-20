using System;
using Foundation.Configuration;

namespace DataCommander.Providers.SqlServer;

internal static class KeyWordRepository
{
    private static string[]? _keyWords;

    public static string[] Get()
    {
        if (_keyWords == null)
        {
            var path = ConfigurationNodeName.FromType(typeof(SqlServerProvider));
            var folder = Settings.SelectNode(path, true);
            _keyWords = folder.Attributes["TSqlKeyWords"].GetValue<string[]>()!;
            Array.Sort(_keyWords, StringComparer.InvariantCultureIgnoreCase);
        }

        return _keyWords;
    }
}