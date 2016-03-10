namespace DataCommander.Foundation.IO
{
    using System.Text;

    internal sealed class DefaultFormatter : IFormatter
    {
        private static readonly DefaultFormatter instance = new DefaultFormatter();

        public static DefaultFormatter Instance => instance;

        void IFormatter.AppendTo(StringBuilder sb, object[] args)
        {
            sb.Append(args[0]);
        }
    }
}