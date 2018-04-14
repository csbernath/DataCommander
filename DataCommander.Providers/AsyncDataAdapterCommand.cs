using System.Data;

namespace DataCommander.Providers
{
    internal sealed class AsyncDataAdapterCommand
    {
        public readonly int LineIndex;
        public readonly string Text;
        public readonly IDbCommand Command;
        public readonly QueryConfiguration.Query Query;
        public readonly string CommandText;

        public AsyncDataAdapterCommand(int lineIndex, string text, IDbCommand command, QueryConfiguration.Query query, string commandText)
        {
            LineIndex = lineIndex;
            Text = text;
            Command = command;
            Query = query;
            CommandText = commandText;
        }
    }
}