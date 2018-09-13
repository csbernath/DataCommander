using Foundation.Collections.ReadOnly;

namespace DataCommander.Providers.QueryConfiguration
{
    public class Query
    {
        public readonly string Name;
        public readonly string Using;
        public readonly string Namespace;
        public readonly ReadOnlyList<string> Results;
        public readonly int? CommandTimeout;

        public Query(string name, string @using, string @namespace, ReadOnlyList<string> results, int? commandTimeout)
        {
            Name = name;
            Using = @using;
            Namespace = @namespace;
            Results = results;
            CommandTimeout = commandTimeout;
        }
    }
}