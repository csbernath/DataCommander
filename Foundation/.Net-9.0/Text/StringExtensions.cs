using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Foundation.Collections.ReadOnly;

namespace Foundation.Text;

public static class StringExtensions
{
    public static ReadOnlyCollection<Line> ToLines(this string text, string indentation)
    {
        List<Line> lines = [];
        using (StringReader stringReader = new StringReader(text))
        {
            while (true)
            {
                if (stringReader.Peek() == -1)
                    break;

                string? lineString = stringReader.ReadLine();
                Line line = lineString!.ToLine(indentation);
                lines.Add(line);
            }
        }

        return lines.ToReadOnlyCollection();
    }

    private static Line ToLine(this string lineString, string indentation)
    {
        string text = lineString;
        int indentationCount = 0;

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