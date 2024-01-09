using System.Collections.ObjectModel;

namespace DataCommander.Api.QueryConfiguration;

public class Query(string name, string @using, string @namespace, ReadOnlyCollection<string> results, int? commandTimeout)
{
    public readonly string Name = name;
    public readonly string Using = @using;
    public readonly string Namespace = @namespace;
    public readonly ReadOnlyCollection<string> Results = results;
    public readonly int? CommandTimeout = commandTimeout;
}