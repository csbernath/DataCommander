using System.Text;

namespace Foundation.IO
{
    internal sealed class AsyncTextWriterListItem
    {
        private readonly IFormatter _formatter;
        private readonly object[] _args;

        public AsyncTextWriterListItem(IFormatter formatter, params object[] args)
        {
            _formatter = formatter;
            _args = args;
        }

        public void AppendTo(StringBuilder sb)
        {
            _formatter.AppendTo(sb, _args);
        }
    }
}