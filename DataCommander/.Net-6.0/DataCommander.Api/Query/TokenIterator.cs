using System.Linq;
using System.Text;

namespace DataCommander.Api.Query;

public sealed class TokenIterator
{
    #region Private Fields

    private static readonly char[] OperatorsOrPunctuators = new[]
    {
        '{', '}', '[', ']', '(', ')', '.', ',', ':', ';', '+', '-', '*', '/', '%', '&', '|', '^', '!', '~', '=',
        '<', '>', '?'
    };

    private readonly string _text;
    private int _index = 0;
    private readonly int _length;
    private int _tokenIndex;
    private int _lineIndex;

    #endregion

    public TokenIterator(string text)
    {
        _text = text;

        if (text != null)
            _length = text.Length;
        else
            _length = 0;
    }

    public Token Next()
    {
        Token token = null;

        while (_index < _length)
        {
            int startPosition;
            int endPosition;
            string? value;
            var c = _text[_index];

            if (c == 'N')
            {
                startPosition = _index;
                if (_index + 1 < _length && _text[_index + 1] == '\'')
                {
                    _index++;
                    value = ReadString();
                    endPosition = _index;
                    token = new Token(_tokenIndex, startPosition, endPosition - 1, _lineIndex, TokenType.String,
                        value);
                }
                else
                {
                    value = ReadKeyWord();
                    endPosition = _index;
                    token = new Token(_tokenIndex, startPosition, endPosition - 1, _lineIndex, TokenType.KeyWord,
                        value);
                }

                break;
            }
            else if (char.IsLetter(c) || c == '[' || c == '@')
            {
                startPosition = _index;
                value = ReadKeyWord();
                endPosition = _index;
                token = new Token(_tokenIndex, startPosition, endPosition - 1, _lineIndex, TokenType.KeyWord,
                    value);
                break;
            }
            else if (c == '"' || c == '\'')
            {
                startPosition = _index;
                value = ReadString();
                endPosition = _index;
                token = new Token(_tokenIndex, startPosition, endPosition - 1, _lineIndex, TokenType.String, value);
                break;
            }
            else if (char.IsDigit(c))
            {
                startPosition = _index;
                value = ReadDigit();
                endPosition = _index;
                token = new Token(_tokenIndex, startPosition, endPosition - 1, _lineIndex, TokenType.Digit, value);
                break;
            }
            else if (OperatorsOrPunctuators.Contains(c))
            {
                startPosition = _index;
                value = c.ToString();
                endPosition = _index;
                token = new Token(_tokenIndex, startPosition, endPosition, _lineIndex,
                    TokenType.OperatorOrPunctuator, value);
                _index++;
                break;
            }
            else if (c == '\r')
            {
                _lineIndex++;
                _index += 2;
            }
            else
                _index++;
        }

        if (token != null)
            _tokenIndex++;

        return token;
    }

    #region Private Methods

    private string? ReadKeyWord()
    {
        var sb = new StringBuilder();

        while (_index < _length)
        {
            var c = _text[_index];
            if (char.IsWhiteSpace(c) || c == ',' || c == '(' || c == ')' || c == '=' || c == '+' || c == '*')
                break;
            else
                _index++;

            sb.Append(c);
        }

        var keyWord = sb.ToString();
        return keyWord;
    }

    private string? ReadString()
    {
        var sb = new StringBuilder();
        _index++;
        var escape = false;

        while (_index < _length)
        {
            var c = _text[_index];
            _index++;

            if (escape)
            {
                if (c == 'n')
                    c = '\n';
                else if (c == 'r')
                    c = '\r';
                else if (c == 't')
                    c = '\t';

                sb.Append(c);

                escape = false;
            }
            else if (c == '"' || c == '\'')
                break;
            else if (c == '\\')
                escape = true;
            else
                sb.Append(c);
        }

        return sb.ToString();
    }

    private string? ReadDigit()
    {
        var sb = new StringBuilder();

        while (_index < _length)
        {
            var c = _text[_index];
            if (!char.IsDigit(c))
                break;
            else
                _index++;
            sb.Append(c);
        }

        return sb.ToString();
    }

    #endregion
}