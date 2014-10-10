using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataCommander.Providers
{
    internal class IdentifierParser
    {
        private TextReader textReader;

        public IdentifierParser(TextReader textReader)
        {
            this.textReader = textReader;
        }

        public IEnumerable<string> Parse()
        {
            while (true)
            {
                int peek = textReader.Peek();

                if (peek == -1)
                    break;

                char peekChar = (char) peek;

                if (peekChar == '[')
                {
                    yield return this.ReadQuotedIdentifier();
                }
                else
                {
                    yield return this.ReadUnquotedIdentifier();
                }
            }
        }

        private string ReadQuotedIdentifier()
        {
            textReader.Read();
            var identifier = new StringBuilder();

            while (true)
            {
                int read = textReader.Read();

                if (read == -1)
                    break;

                char readChar = (char) read;

                if (readChar == ']')
                {
                    textReader.Read();
                    break;
                }
                else
                {
                    identifier.Append(readChar);
                }
            }

            return identifier.ToString();
        }

        private string ReadUnquotedIdentifier()
        {
            var identifier = new StringBuilder();

            while (true)
            {
                int read = textReader.Read();

                if (read == -1)
                    break;

                char readChar = (char) read;

                if (readChar == '.')
                {
                    break;
                }
                else
                {
                    identifier.Append(readChar);
                }
            }

            return identifier.ToString();
        }
    }
}