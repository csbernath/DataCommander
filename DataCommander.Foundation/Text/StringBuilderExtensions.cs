using System.Collections.Generic;
using System.Text;

namespace Foundation.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="separator"></param>
        /// <param name="values"></param>
        public static void Append(this StringBuilder stringBuilder, string separator, IEnumerable<string> values)
        {
            var first = true;
            foreach (var value in values)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.Append(separator);

                stringBuilder.Append(value);
            }
        }
    }
}