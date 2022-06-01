using System;
using Foundation.Assertions;

namespace Foundation.Log;

public static class LogFactory
{
    private static ILogFactory _logFactory = NullLogFactory.Instance;

    public static ILogFactory Instance => _logFactory;

    public static void Set(ILogFactory logFactory)
    {
        ArgumentNullException.ThrowIfNull(logFactory);
        _logFactory = logFactory;
    }
}