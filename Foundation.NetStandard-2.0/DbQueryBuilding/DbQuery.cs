using System.Collections.ObjectModel;

namespace Foundation.DbQueryBuilding
{
    public class DbQuery
    {
        public readonly string Using;
        public readonly string Namespace;
        public readonly string Name;
        public readonly string CommandText;
        public readonly int CommandTimeout;
        public readonly ReadOnlyCollection<DbQueryParameter> Parameters;
        public readonly ReadOnlyCollection<DbQueryResult> Results;

        public DbQuery(string @using, string @namespace, string name, string commandText, int commandTimeout, ReadOnlyCollection<DbQueryParameter> parameters,
            ReadOnlyCollection<DbQueryResult> results)
        {
            Using = @using;
            Namespace = @namespace;
            Name = name;
            CommandText = commandText;
            CommandTimeout = commandTimeout;
            Parameters = parameters;
            Results = results;
        }
    }
}