using System.Data;

namespace DataCommander.Providers
{
    internal sealed class AsyncDataAdapterCommand
    {
        public readonly int LineIndex;
        public readonly IDbCommand Command;

        public AsyncDataAdapterCommand(int lineIndex, IDbCommand command)
        {
            LineIndex = lineIndex;
            Command = command;
        }
    }
}