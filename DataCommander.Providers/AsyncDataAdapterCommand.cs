namespace DataCommander.Providers
{
    using System.Data;

    internal sealed class AsyncDataAdapterCommand
    {
        public int LineIndex;
        public IDbCommand Command;
    }
}