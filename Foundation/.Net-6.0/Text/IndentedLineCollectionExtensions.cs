using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Text;

public static class IndentedLineCollectionExtensions
{
    [Pure]
    public static string ToIndentedString(this IEnumerable<Line> lines, string indentation)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var stringBuilder = new StringBuilder();
        var first = true;
        foreach (var indentedLine in lines)
        {
            if (first)
                first = false;
            else
                stringBuilder.Append("\r\n");

            for (var i = 0; i < indentedLine.Indentation; ++i)
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

        var first = true;
        foreach (var lineGroup in lineGroups)
        {
            if (!first)
                yield return separator;

            foreach (var line in lineGroup)
            {
                if (first)
                    first = false;

                yield return line;
            }
        }
    }
}