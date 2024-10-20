using System.Diagnostics;

namespace DataCommander.Api.Query;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Token(int index, int startPosition, int endPosition, int lineIndex, TokenType type, string value)
{
    public readonly int Index = index;
    public readonly int StartPosition = startPosition;
    public readonly int EndPosition = endPosition;
    public readonly int LineIndex = lineIndex;
    public readonly TokenType Type = type;
    public readonly string Value = value;

    private string DebuggerDisplay => $"{Value} ({Type})";
}