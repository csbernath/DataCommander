using System.Collections.ObjectModel;

namespace Foundation.DbQueryBuilding
{
    public sealed class DbQueryResult
    {
        public readonly string Name;
        public readonly ReadOnlyCollection<DbQueryResultField> Fields;

        public DbQueryResult(string name, ReadOnlyCollection<DbQueryResultField> fields)
        {
            Name = name;
            Fields = fields;
        }
    }
}