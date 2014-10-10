namespace DataCommander.Foundation.IO
{
    using System;
    using System.Text;

    internal sealed class AsyncTextWriterListItem
    {
        private readonly IFormatter formatter;
        private readonly Object[] args;

        public AsyncTextWriterListItem(IFormatter formatter, params Object[] args)
        {
            this.formatter = formatter;
            this.args = args;
        }

        public void AppendTo(StringBuilder sb)
        {
            this.formatter.AppendTo(sb, this.args);
        }
    }
}