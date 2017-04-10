namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal sealed class IdentifierParser
    {
        private readonly TextReader textReader;

        public IdentifierParser(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public IEnumerable<string> Parse()
        {
            var peekChar = default(char);

            while (true)
            {
                var peek = this.textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                peekChar = (char)peek;

                if (peekChar == '.')
                {
                    this.textReader.Read();
                }
                else if (peekChar == '\'')
                {
                    yield return this.ReadQuotedIdentifier();
                }
                else
                {
                    yield return this.ReadUnquotedIdentifier();
                }
            }

            if (peekChar == '.')
            {
                yield return null;
            }
        }

        #region Private Methods

        private string ReadQuotedIdentifier()
        {
            this.textReader.Read();
            var identifier = new StringBuilder();

            while (true)
            {
                var peek = this.textReader.Peek();

                if (peek == -1)
                    break;

                var peekChar = (char)peek;

                if (peekChar == '\'')
                {
                    this.textReader.Read();
                    break;
                }
                else
                {
                    identifier.Append(peekChar);
                    this.textReader.Read();
                }
            }

            return identifier.ToString();
        }

        private string ReadUnquotedIdentifier()
        {
            var identifier = new StringBuilder();

            while (true)
            {
                var peek = this.textReader.Peek();

                if (peek == -1)
                    break;

                var peekChar = (char)peek;

                if (peekChar == '.')
                {
                    break;
                }
                else
                {
                    identifier.Append(peekChar);
                    this.textReader.Read();
                }
            }

            return identifier.ToString();
        }

        #endregion
    }
}