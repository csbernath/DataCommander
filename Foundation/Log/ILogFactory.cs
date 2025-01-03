using System;

namespace Foundation.Log;

public interface ILogFactory : IDisposable
{
    string? FileName { get; }
    ILog GetLog(string? name);
}