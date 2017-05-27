namespace DataCommander.Providers.SqlServer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal sealed class IdentifierParser
    {
        private readonly TextReader _textReader;

        public IdentifierParser(TextReader textReader)
        {
            _textReader = textReader;
        }

        public IEnumerable<string> Parse()
        {
            var peekChar = default(char);

            while (true)
            {
                var peek = _textReader.Peek();

                if (peek == -1)
                {
                    break;
                }

                peekChar = (char)peek;

                if (peekChar == '.')
                {
                    _textReader.Read();
                }
                else if (peekChar == '[')
                {
                    yield return ReadQuotedIdentifier();
                }
                else
                {
                    yield return ReadUnquotedIdentifier();
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
            _textReader.Read();
            var identifier = new StringBuilder();

            while (true)
            {
                var peek = _textReader.Peek();

                if (peek == -1)
                    break;

                var peekChar = (char)peek;

                if (peekChar == ']')
                {
                    _textReader.Read();
                    break;
                }
                else
                {
                    identifier.Append(peekChar);
                    _textReader.Read();
                }
            }

            return identifier.ToString();
        }

        private string ReadUnquotedIdentifier()
        {
            var identifier = new StringBuilder();

            while (true)
            {
                var peek = _textReader.Peek();

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
                    _textReader.Read();
                }
            }

            return identifier.ToString();
        }

        #endregion
    }
}