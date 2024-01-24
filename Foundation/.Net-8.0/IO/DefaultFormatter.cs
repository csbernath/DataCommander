using System.Text;

namespace Foundation.IO;

internal sealed class DefaultFormatter : IFormatter
{
    public static DefaultFormatter Instance { get; } = new();

    void IFormatter.AppendTo(StringBuilder stringBuilder, object[] args) => stringBuilder.Append(args[0]);
}