namespace DataCommander.Providers
{
    public sealed class Token
    {
        private readonly int index;
        private readonly int startPosition;
        private readonly int endPosition;
        private readonly int lineIndex;
        private readonly TokenType type;
        private readonly string value;

        public Token(
            int index,
            int startPosition,
            int endPosition,
            int lineIndex,
            TokenType type,
            string value)
        {
            this.index = index;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.lineIndex = lineIndex;
            this.type = type;
            this.value = value;
        }

        public int Index => this.index;

        public int StartPosition => this.startPosition;

        public int EndPosition => this.endPosition;

        public int LineIndex => this.lineIndex;

        public TokenType Type => this.type;

        public string Value => this.value;
    }
}