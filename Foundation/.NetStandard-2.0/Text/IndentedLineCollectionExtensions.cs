using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Foundation.Assertions;
using Foundation.Linq;

namespace Foundation.Text
{
    public static class IndentedLineCollectionExtensions
    {
        [Pure]
        public static string ToIndentedString(this IEnumerable<Line> lines, string indentation)
        {
            Assert.IsNotNull(lines);

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
            Assert.IsNotNull(lineGroups);
            Assert.IsNotNull(separator);

            foreach (var item in lineGroups.SelectIndexed())
            {
                if (item.Index > 0)
                    yield return separator;

                var paragraph = item.Value;
                foreach (var line in paragraph)
                    yield return line;
            }
        }
    }
}