using System.Text;

namespace Foundation.IO
{
    internal sealed class AsyncTextWriterListItem
    {
        private readonly IFormatter _formatter;
        private readonly object[] _args;

        public AsyncTextWriterListItem(IFormatter formatter, params object[] args)
        {
            this._formatter = formatter;
            this._args = args;
        }

        public void AppendTo(StringBuilder sb)
        {
            this._formatter.AppendTo(sb, this._args);
        }
    }
}