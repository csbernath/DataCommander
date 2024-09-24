using System.Text;

namespace Foundation.IO;

internal sealed class AsyncTextWriterListItem(IFormatter formatter, params object[] args)
{
    public void AppendTo(StringBuilder sb) => formatter.AppendTo(sb, args);
}