using System.Collections.Generic;
using System.Data.Common;
using Foundation.Data.SqlClient.DbQueryBuilding;

namespace DataCommander.Application;

internal sealed class AsyncDataAdapterCommand(
    string? fileName,
    int? lineIndex,
    DbCommand command,
    Api.QueryConfiguration.Query? query,
    IReadOnlyCollection<DbRequestParameter>? parameters,
    string? commandText)
{
    public readonly string? FileName = fileName;
    public readonly int? LineIndex = lineIndex;
    public readonly DbCommand Command = command;
    public readonly Api.QueryConfiguration.Query? Query = query;
    public readonly IReadOnlyCollection<DbRequestParameter>? Parameters = parameters;
    public readonly string? CommandText = commandText;
}