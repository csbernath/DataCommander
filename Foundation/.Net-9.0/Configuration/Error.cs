using System;

namespace Foundation.Configuration;

internal sealed class Error(ErrorType type, string message, Exception exception)
{
    public ErrorType Type { get; } = type;

    public override string ToString() => $"{Type}\r\n{message}\r\n{exception}";
}