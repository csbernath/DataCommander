using System.Collections.Generic;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public class DbRequest(
    string directory,
    string name,
    string @using,
    string @namespace,
    string commandText,
    int? commandTimeout,
    IReadOnlyCollection<DbRequestParameter> parameters,
    IReadOnlyCollection<DbQueryResult> results)
{
    public readonly string Directory = directory;
    public readonly string Name = name;
    public readonly string Using = @using;
    public readonly string Namespace = @namespace;
    public readonly string CommandText = commandText;
    public readonly int? CommandTimeout = commandTimeout;
    public readonly IReadOnlyCollection<DbRequestParameter> Parameters = parameters;
    public readonly IReadOnlyCollection<DbQueryResult> Results = results;
}