using System.Collections.Generic;
using System.IO;

namespace Foundation.Text;

public static class StringExtensions
{
    public static IReadOnlyCollection<Line> ToLines(this string text, string indentation)
    {
        List<Line> lines = [];
        using (var stringReader = new StringReader(text))
        {
            while (true)
            {
                if (stringReader.Peek() == -1)
                    break;

                var lineString = stringReader.ReadLine();
                var line = lineString!.ToLine(indentation);
                lines.Add(line);
            }
        }

        return lines.ToArray();
    }

    private static Line ToLine(this string lineString, string indentation)
    {
        var text = lineString;
        var indentationCount = 0;

        while (true)
        {
            if (text.StartsWith(indentation))
            {
                text = text[indentation.Length..];
                ++indentationCount;
            }
            else
                break;
        }

        return new Line(indentationCount, text);
    }
}