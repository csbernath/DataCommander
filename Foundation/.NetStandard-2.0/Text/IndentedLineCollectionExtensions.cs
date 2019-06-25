using System.Collections.Generic;
using System.Text;
using Foundation.Assertions;

namespace Foundation.Text
{
    public static class IndentedLineCollectionExtensions
    {
        public static string ToString(this IEnumerable<IndentedLine> indentedLines, string indentation)
        {
            Assert.IsNotNull(indentedLines);

            var stringBuilder = new StringBuilder();
            var first = true;
            foreach (var indentedLine in indentedLines)
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