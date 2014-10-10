namespace DataCommander.Providers
{
    public sealed class Token
    {
        private int index;
        private int startPosition;
        private int endPosition;
        private TokenType type;
        private string value;

        public Token(
            int index,
            int startPosition,
            int endPosition,
            TokenType type,
            string value )
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
                return startPosition;
            }
        }

        public int EndPosition
        {
            get
            {
                return endPosition;
            }
        }

        public TokenType Type
        {
            get
            {
                return type;
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