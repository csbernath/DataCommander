namespace DataCommander.Foundation.IO
{
    using System.Text;

    internal sealed class AsyncTextWriterListItem
    {
        private readonly IFormatter formatter;
        private readonly object[] args;

        public AsyncTextWriterListItem(IFormatter formatter, params object[] args)
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