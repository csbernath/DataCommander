namespace DataCommander.Api;

public sealed class Statement
{
    public readonly int LineIndex;
    public readonly string CommandText;

    public Statement(int lineIndex, string commandText)
    {
        LineIndex = lineIndex;
        CommandText = commandText;
    }
}