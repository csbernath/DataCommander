using System;

namespace Foundation.Log;

public sealed class NullLogFactory : ILogFactory
{
    public static readonly NullLogFactory Instance = new();

    private NullLogFactory()
    {
    }

    string? ILogFactory.FileName => null;

    ILog ILogFactory.GetLog(string? name) => NullLog.Instance;

    void IDisposable.Dispose()
    {
    }
}