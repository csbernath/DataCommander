namespace DataCommander.Foundation.IO
{
    using System.Text;

    internal sealed class DefaultFormatter : IFormatter
    {
        public static DefaultFormatter Instance { get; } = new DefaultFormatter();

        void IFormatter.AppendTo(StringBuilder sb, object[] args)
        {
            sb.Append(args[0]);
        }
    }
}