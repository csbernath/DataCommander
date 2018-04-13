using System.Data;

namespace DataCommander.Providers
{
    internal sealed class AsyncDataAdapterCommand
    {
        public readonly int LineIndex;
        public readonly IDbCommand Command;
        public readonly QueryConfiguration.Query Query;
        public readonly string CommandText;

        public AsyncDataAdapterCommand(int lineIndex, IDbCommand command, QueryConfiguration.Query query, string commandText)
        {
            LineIndex = lineIndex;
            Command = command;
            Query = query;
            CommandText = commandText;
        }
    }
}