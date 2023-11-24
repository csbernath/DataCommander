using System.Diagnostics;

namespace DataCommander.Api.Query;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Token
{
    public readonly int Index;
    public readonly int StartPosition;
    public readonly int EndPosition;
    public readonly int LineIndex;
    public readonly TokenType Type;
    public readonly string? Value;

    public Token(int index, int startPosition, int endPosition, int lineIndex, TokenType type, string? value)
    {
        Index = index;
        StartPosition = startPosition;
        EndPosition = endPosition;
        LineIndex = lineIndex;
        Type = type;
        Value = value;
    }

    private string DebuggerDisplay => $"{Value} ({Type})";
}