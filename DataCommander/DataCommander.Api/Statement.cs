namespace DataCommander.Api;

public sealed class Statement(int lineIndex, string commandText)
{
    public readonly int LineIndex = lineIndex;
    public readonly string CommandText = commandText;
}