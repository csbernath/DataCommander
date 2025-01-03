using System.Collections.ObjectModel;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public class DbRequest(
    string directory,
    string name,
    string @using,
    string @namespace,
    string commandText,
    int? commandTimeout,
    ReadOnlyCollection<DbRequestParameter> parameters,
    ReadOnlyCollection<DbQueryResult> results)
{
    public readonly string Directory = directory;
    public readonly string Name = name;
    public readonly string Using = @using;
    public readonly string Namespace = @namespace;
    public readonly string CommandText = commandText;
    public readonly int? CommandTimeout = commandTimeout;
    public readonly ReadOnlyCollection<DbRequestParameter> Parameters = parameters;
    public readonly ReadOnlyCollection<DbQueryResult> Results = results;
}