using System.Collections.ObjectModel;
using System.Data;
using Foundation.Collections;
using Foundation.Data.DbQueryBuilding;

namespace DataCommander.Providers
{
    internal sealed class AsyncDataAdapterCommand
    {
        public readonly string FileName;
        public readonly int LineIndex;
        public readonly string Text;
        public readonly IDbCommand Command;
        public readonly QueryConfiguration.Query Query;
        public readonly ReadOnlyList<DbRequestParameter> Parameters;
        public readonly string CommandText;

        public AsyncDataAdapterCommand(string fileName, int lineIndex, string text, IDbCommand command, QueryConfiguration.Query query,
            ReadOnlyList<DbRequestParameter> parameters, string commandText)
        {
            LineIndex = lineIndex;
            Text = text;
            Command = command;
            Query = query;
            Parameters = parameters;
            CommandText = commandText;
            FileName = fileName;
        }
    }
}