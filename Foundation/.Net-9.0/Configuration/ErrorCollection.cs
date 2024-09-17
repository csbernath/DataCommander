using System.Collections.ObjectModel;
using System.Text;

namespace Foundation.Configuration;

internal sealed class ErrorCollection : Collection<Error>
{
    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (Error error in this)
            stringBuilder.AppendLine(error.ToString());

        return stringBuilder.ToString();
    }
}