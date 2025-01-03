using System.Collections.ObjectModel;
using System.Data.Common;
using Foundation.Data.SqlClient.DbQueryBuilding;

namespace DataCommander.Application;

internal sealed class AsyncDataAdapterCommand
{
    public readonly string FileName;
    public readonly int LineIndex;
    public readonly DbCommand Command;
    public readonly Api.QueryConfiguration.Query Query;
    public readonly ReadOnlyCollection<DbRequestParameter> Parameters;
    public readonly string CommandText;

    public AsyncDataAdapterCommand(string fileName, int lineIndex, DbCommand command, Api.QueryConfiguration.Query query,
        ReadOnlyCollection<DbRequestParameter> parameters, string commandText)
    {
        FileName = fileName;
        LineIndex = lineIndex;
        Command = command;
        Query = query;
        Parameters = parameters;
        CommandText = commandText;
    }
}