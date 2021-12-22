using System;

namespace Foundation.Configuration;

internal sealed class Error
{
    private readonly string _message;
    private readonly Exception _exception;

    public Error(ErrorType type, string message, Exception exception)
    {
        Type = type;
        _message = message;
        _exception = exception;
    }

    public ErrorType Type { get; }

    public override string ToString()
    {
        return $"{Type}\r\n{_message}\r\n{_exception}";
    }
}