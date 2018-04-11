using System.Collections.ObjectModel;

namespace Foundation.Data.SqlClient.Orm
{
    public class Query
    {
        public readonly string Namespace;
        public readonly string Name;
        public readonly string CommandText;
        public readonly int CommandTimeout;
        public readonly ReadOnlyCollection<Parameter> Parameters;
        public readonly ReadOnlyCollection<Result> Results;

        public Query(string @namespace, string name, string commandText, int commandTimeout, ReadOnlyCollection<Parameter> parameters, ReadOnlyCollection<Result> results)
        {
            Namespace = @namespace;
            Name = name;
            CommandText = commandText;
            CommandTimeout = commandTimeout;
            Parameters = parameters;
            Results = results;
        }
    }
}