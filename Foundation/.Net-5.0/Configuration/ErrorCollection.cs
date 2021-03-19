using System.Collections.ObjectModel;
using System.Text;

namespace Foundation.Configuration
{
    internal sealed class ErrorCollection : Collection<Error>
    {
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            foreach (var error in this)
                stringBuilder.AppendLine(error.ToString());

            return stringBuilder.ToString();
        }
    }
}