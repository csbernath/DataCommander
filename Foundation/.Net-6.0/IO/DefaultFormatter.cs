using System.Text;

namespace Foundation.IO
{
    internal sealed class DefaultFormatter : IFormatter
    {
        public static DefaultFormatter Instance { get; } = new();

        void IFormatter.AppendTo(StringBuilder sb, object[] args)
        {
            sb.Append(args[0]);
        }
    }
}