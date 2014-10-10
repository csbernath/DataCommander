using System.Linq;
using System.Text;

namespace DataCommander.Providers
{
    internal sealed class TokenIterator
    {
        #region Private Fields

        private static char[] operatorsOrPunctuators = new char[]
        {
            '{','}','[',']','(',')','.',',',':',';','+','-','*','/','%','&','|','^','!','~','=','<','>','?'
        };

        private string text;
        private int index = 0;
        private int length;
        private int tokenIndex;

        #endregion

        public TokenIterator( string text )
        {
            this.text = text;

            if (text != null)
            {
                this.length = text.Length;
            }
            else
            {
                this.length = 0;
            }
        }

        public Token Next()
        {
            Token token = null;
            int startPosition;
            int endPosition;
            string value;

            while (this.index < this.length)
            {
                char c = text[ this.index ];

                if (c == 'N')
                {
                    startPosition = this.index;
                    if (this.index + 1 < this.length && text[ this.index + 1 ] == '\'')
                    {
                        this.index++;
                        value = this.ReadString();
                        endPosition = this.index;
                        token = new Token( this.tokenIndex, startPosition, endPosition - 1, TokenType.String, value );
                    }
                    else
                    {
                        value = this.ReadKeyWord();
                        endPosition = this.index;
                        token = new Token( this.tokenIndex, startPosition, endPosition - 1, TokenType.KeyWord, value );
                    }
                    break;
                }
                else if (char.IsLetter( c ) || c == '[' || c == '@')
                {
                    startPosition = this.index;
                    value = this.ReadKeyWord();
                    endPosition = this.index;
                    token = new Token( this.tokenIndex, startPosition, endPosition - 1, TokenType.KeyWord, value );
                    break;
                }
                else if (c == '"' || c == '\'')
                {
                    startPosition = this.index;
                    value = this.ReadString();
                    endPosition = this.index;
                    token = new Token( this.tokenIndex, startPosition, endPosition - 1, TokenType.String, value );
                    break;
                }
                else if (char.IsDigit( c ) || c == '-')
                {
                    startPosition = this.index;
                    value = this.ReadDigit();
                    endPosition = this.index;
                    token = new Token( this.tokenIndex, startPosition, endPosition - 1, TokenType.Digit, value );
                    break;
                }
                else if (operatorsOrPunctuators.Contains(c))
                {
                    startPosition = this.index;
                    value = c.ToString();
                    endPosition = this.index;
                    token = new Token( this.tokenIndex, startPosition, endPosition, TokenType.OperatorOrPunctuator, value );
                    this.index++;
                    break;
                }
                else
                {
                    this.index++;
                }
            }

            if (token != null)
            {
                this.tokenIndex++;
            }
            return token;
        }

        #region Private Methods

        private string ReadKeyWord()
        {
            var keyWord = new StringBuilder();

            while (this.index < length)
            {
                char c = text[this.index];
                if (char.IsWhiteSpace(c) || c == ',' || c == '(' || c == ')' || c == '=')
                {
                    break;
                }
                else
                {
                    this.index++;
                }
                keyWord.Append(c);
            }

            string keyWord2 = keyWord.ToString();

            return keyWord2;
        }

        private string ReadString()
        {
            var keyWord = new StringBuilder();
            this.index++;
            bool escape = false;

            while (this.index < length)
            {
                char c = text[this.index];
                this.index++;

                if (escape)
                {
                    if (c == 'n')
                    {
                        c = '\n';
                    }
                    else if (c == 'r')
                    {
                        c = '\r';
                    }
                    else if (c == 't')
                    {
                        c = '\t';
                    }

                    keyWord.Append(c);

                    escape = false;
                }
                else if (c == '"' || c == '\'')
                {
                    break;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else
                {
                    keyWord.Append(c);
                }
            }

            return keyWord.ToString();
        }

        private string ReadDigit()
        {
            var digit = new StringBuilder();

            while (this.index < length)
            {
                char c = text[this.index];
                if (char.IsWhiteSpace(c) || c == ',')
                {
                    break;
                }
                else
                {
                    this.index++;
                }
                digit.Append(c);
            }

            return digit.ToString();
        }

        #endregion
    }
}