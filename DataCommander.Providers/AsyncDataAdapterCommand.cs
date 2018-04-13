using System.Data;

namespace DataCommander.Providers
{
    internal sealed class AsyncDataAdapterCommand
    {
        public readonly int LineIndex;
        public readonly IDbCommand Command;
        public readonly QueryConfiguration.Query Query;

        public AsyncDataAdapterCommand(int lineIndex, IDbCommand command, QueryConfiguration.Query query)
        {
            LineIndex = lineIndex;
            Command = command;
            Query = query;
        }
    }
}