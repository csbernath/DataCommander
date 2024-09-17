using System.Collections.Generic;
using System.Text;

namespace Foundation.Text;

public static class StringBuilderExtensions
{
    public static void Append(this StringBuilder stringBuilder, string separator, IEnumerable<string> values)
    {
        bool first = true;
        foreach (string value in values)
        {
            if (first)
                first = false;
            else
                stringBuilder.Append(separator);

            stringBuilder.Append(value);
        }
    }
}