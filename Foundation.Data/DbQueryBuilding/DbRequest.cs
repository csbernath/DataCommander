using System.Collections.ObjectModel;

namespace Foundation.Data.DbQueryBuilding
{
    public class DbRequest
    {
        public readonly string Directory;
        public readonly string Name;
        public readonly string Using;
        public readonly string Namespace;
        public readonly string CommandText;
        public readonly int? CommandTimeout;
        public readonly ReadOnlyCollection<DbRequestParameter> Parameters;
        public readonly ReadOnlyCollection<DbQueryResult> Results;

        public DbRequest(string directory, string name, string @using, string @namespace, string commandText, int? commandTimeout, ReadOnlyCollection<DbRequestParameter> parameters,
            ReadOnlyCollection<DbQueryResult> results)
        {
            Using = @using;
            Namespace = @namespace;
            Name = name;
            CommandText = commandText;
            CommandTimeout = commandTimeout;
            Parameters = parameters;
            Results = results;
            Directory = directory;
        }
    }
}