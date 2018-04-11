using System.Collections.ObjectModel;

namespace DataCommander.Providers.ResultWriter.QueryConfiguration
{
    public class Query
    {
        public readonly string Namespace;
        public readonly string Name;
        public readonly ReadOnlyCollection<Parameter> Parameters;
        public readonly ReadOnlyCollection<string> Results;

        public Query(string @namespace, string name, ReadOnlyCollection<Parameter> parameters, ReadOnlyCollection<string> results)
        {
            Namespace = @namespace;
            Name = name;
            Parameters = parameters;
            Results = results;
        }
    }
}