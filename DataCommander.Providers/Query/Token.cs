namespace DataCommander.Providers
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
            this.Index = index;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
            this.LineIndex = lineIndex;
            this.Type = type;
            this.Value = value;
        }

        public int Index { get; }

        public int StartPosition { get; }

        public int EndPosition { get; }

        public int LineIndex { get; }

        public TokenType Type { get; }

        public string Value { get; }
    }
}