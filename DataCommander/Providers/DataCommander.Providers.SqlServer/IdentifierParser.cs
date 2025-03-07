﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataCommander.Providers.SqlServer;

internal sealed class IdentifierParser(TextReader textReader)
{
    public IEnumerable<string?> Parse()
    {
        var peekChar = default(char);

        while (true)
        {
            var peek = textReader.Peek();

            if (peek == -1)
                break;

            peekChar = (char) peek;

            if (peekChar == '.')
                textReader.Read();
            else if (peekChar == '[')
                yield return ReadQuotedIdentifier();
            else
                yield return ReadUnquotedIdentifier();
        }

        if (peekChar == '.') yield return null;
    }

    private string ReadQuotedIdentifier()
    {
        textReader.Read();
        var identifier = new StringBuilder();

        while (true)
        {
            var peek = textReader.Peek();

            if (peek == -1)
                break;

            var peekChar = (char) peek;

            if (peekChar == ']')
            {
                textReader.Read();
                break;
            }

            identifier.Append(peekChar);
            textReader.Read();
        }

        return identifier.ToString();
    }

    private string ReadUnquotedIdentifier()
    {
        var identifier = new StringBuilder();

        while (true)
        {
            var peek = textReader.Peek();

            if (peek == -1)
                break;

            var peekChar = (char) peek;

            if (peekChar == '.')
            {
                break;
            }

            identifier.Append(peekChar);
            textReader.Read();
        }

        return identifier.ToString();
    }
}