namespace DataCommander.Providers
{
    public sealed class Token
    {
        private readonly int index;
        private readonly int startPosition;
        private readonly int endPosition;
        private readonly TokenType type;
        private readonly string value;

        public Token(
            int index,
            int startPosition,
            int endPosition,
            TokenType type,
            string value)
        {
            this.index = index;
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.type = type;
            this.value = value;
        }

        public int Index
        {
            get
            {
                return this.index;
            }
        }

        public int StartPosition
        {
            get
            {
                return this.startPosition;
            }
        }

        public int EndPosition
        {
            get
            {
                return this.endPosition;
            }
        }

        public TokenType Type
        {
            get
            {
                return this.type;
            }
        }

        public string Value
        {
            get
            {
                return this.value;
            }
        }
    }
}