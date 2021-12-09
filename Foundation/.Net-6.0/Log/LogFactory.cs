﻿using Foundation.Assertions;

namespace Foundation.Log;

public static class LogFactory
{
    private static ILogFactory _logFactory = NullLogFactory.Instance;

    public static ILogFactory Instance => _logFactory;

    public static void Set(ILogFactory logFactory)
    {
        Assert.IsNotNull(logFactory);
        _logFactory = logFactory;
    }
}