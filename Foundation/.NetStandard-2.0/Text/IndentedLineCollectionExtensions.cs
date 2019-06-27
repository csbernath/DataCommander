using System.Collections.Generic;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Text
{
    public static class IndentedLineCollectionExtensions
    {
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
    }
}