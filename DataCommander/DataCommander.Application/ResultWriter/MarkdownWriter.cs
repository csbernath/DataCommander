using System.Collections.Generic;
using System.IO;
using System.Text;
using Foundation.Core;

namespace DataCommander.Application.ResultWriter;

public class MarkdownWriter(TextWriter textWriter)
{
    private static readonly HashSet<char> SpecialCharacters =
    [

        AsciiCharacter.Asterisk, AsciiCharacter.Backslash, AsciiCharacter.Backtick, AsciiCharacter.Dot, AsciiCharacter.ExclamationMark,
        AsciiCharacter.LeftAngleBracket, AsciiCharacter.LeftBracket, AsciiCharacter.LeftCurlyBrace, AsciiCharacter.LeftParenthesis, AsciiCharacter.MinusSign,
        AsciiCharacter.Pipe, AsciiCharacter.PlusSign, AsciiCharacter.PoundSign, AsciiCharacter.RightAngleBracket, AsciiCharacter.RightBracket,
        AsciiCharacter.RightCurlyBrace, AsciiCharacter.RightParenthesis, AsciiCharacter.Underscore
    ];

    public void BeginTable(IReadOnlyCollection<string?> columnNames)
    {
        WriteRow(columnNames);

        for (var colunIndex = 0; colunIndex < columnNames.Count; ++colunIndex)
        {
            if (colunIndex == 0)
                textWriter.Write('|');
            textWriter.Write("-|");
        }

        textWriter.WriteLine();
    }

    public void WriteRow(IReadOnlyCollection<string?> values)
    {
        var first = true;
        foreach (var value in values)
        {
            if (first)
            {
                first = false;
                textWriter.Write('|');
            }

            if (value != null)
                WriteCell(value);

            textWriter.Write('|');
        }
        
        textWriter.WriteLine();
    }

    private void WriteCell(string value)
    {
        var stringBuilder = new StringBuilder();
        foreach (var character in value)
        {
            if (SpecialCharacters.Contains(character))
                stringBuilder.Append(AsciiCharacter.Backslash);

            stringBuilder.Append(character);
        }

        textWriter.Write(stringBuilder.ToString());
    }
}