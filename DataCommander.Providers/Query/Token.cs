namespace DataCommander.Providers.Query
{
    public sealed class Token
    {
        public Token(
            int index,
            int startPosition,
            int endPosition,
            int lineIndex,
            TokenType type,
            string value)
        {
            Index = index;
            StartPosition = startPosition;
            EndPosition = endPosition;
            LineIndex = lineIndex;
            Type = type;
            Value = value;
        }

        public int Index { get; }
        public int StartPosition { get; }
        public int EndPosition { get; }
        public int LineIndex { get; }
        public TokenType Type { get; }
        public string Value { get; }
    }
}