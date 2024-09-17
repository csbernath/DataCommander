using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Foundation.Text;

public static class IndentedLineCollectionExtensions
{
    [Pure]
    public static string ToIndentedString(this IEnumerable<Line> lines, string indentation)
    {
        ArgumentNullException.ThrowIfNull(lines);

        StringBuilder stringBuilder = new StringBuilder();
        bool first = true;
        foreach (Line indentedLine in lines)
        {
            if (first)
                first = false;
            else
                stringBuilder.Append("\r\n");

            for (int i = 0; i < indentedLine.Indentation; ++i)
                stringBuilder.Append(indentation);

            stringBuilder.Append(indentedLine.Text);
        }

        return stringBuilder.ToString();
    }

    [Pure]
    public static IEnumerable<Line> Join(this IEnumerable<IEnumerable<Line>> lineGroups, Line separator)
    {
        ArgumentNullException.ThrowIfNull(lineGroups);
        ArgumentNullException.ThrowIfNull(separator);

        bool first = true;
        foreach (IEnumerable<Line> lineGroup in lineGroups)
        {
            if (!first)
                yield return separator;

            foreach (Line line in lineGroup)
            {
                if (first)
                    first = false;

                yield return line;
            }
        }
    }
}