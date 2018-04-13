using System.Collections.ObjectModel;

namespace DataCommander.Providers.ResultWriter.QueryConfiguration
{
    public class Query
    {
        public readonly string Using;
        public readonly string Namespace;
        public readonly string Name;
        public readonly ReadOnlyCollection<Parameter> Parameters;
        public readonly ReadOnlyCollection<Result> Results;

        public Query(string @using, string @namespace, string name, ReadOnlyCollection<Parameter> parameters, ReadOnlyCollection<Result> results)
        {
            Using = @using;
            Namespace = @namespace;
            Name = name;
            Parameters = parameters;
            Results = results;
        }
    }
}