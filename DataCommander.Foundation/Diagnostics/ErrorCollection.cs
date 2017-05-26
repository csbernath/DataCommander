using System.Collections.ObjectModel;
using System.Text;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ErrorCollection : Collection<Error>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var error in this)
            {
                sb.AppendLine(error.ToString());
            }

            return sb.ToString();
        }
    }
}