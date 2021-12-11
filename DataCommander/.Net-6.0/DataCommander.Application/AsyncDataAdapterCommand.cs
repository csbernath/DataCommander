using System.Collections.ObjectModel;
using System.Data;
using Foundation.Data.DbQueryBuilding;

namespace DataCommander.Application;

internal sealed class AsyncDataAdapterCommand
{
    public readonly string FileName;
    public readonly int LineIndex;
    public readonly string Text;
    public readonly IDbCommand Command;
    public readonly Api.QueryConfiguration.Query Query;
    public readonly ReadOnlyCollection<DbRequestParameter> Parameters;
    public readonly string CommandText;

    public AsyncDataAdapterCommand(string fileName, int lineIndex, string text, IDbCommand command, Api.QueryConfiguration.Query query,
        ReadOnlyCollection<DbRequestParameter> parameters, string commandText)
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