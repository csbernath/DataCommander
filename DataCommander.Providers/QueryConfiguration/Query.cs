using System.Collections.ObjectModel;

namespace DataCommander.Providers.QueryConfiguration
{
    public class Query
    {
        public readonly string Name;
        public readonly string Using;
        public readonly string Namespace;
        public readonly ReadOnlyCollection<Parameter> Parameters;
        public readonly ReadOnlyCollection<Result> Results;

        public Query(string name, string @using, string @namespace, ReadOnlyCollection<Parameter> parameters, ReadOnlyCollection<Result> results)
        {
            Name = name;
            Using = @using;
            Namespace = @namespace;
            Parameters = parameters;
            Results = results;
        }
    }
}