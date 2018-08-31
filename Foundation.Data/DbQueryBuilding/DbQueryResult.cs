using System.Collections.ObjectModel;
using Foundation.Collections;

namespace Foundation.Data.DbQueryBuilding
{
    public sealed class DbQueryResult
    {
        public readonly string Name;
        public readonly string FieldName;
        public readonly ReadOnlyList<DbQueryResultField> Fields;

        public DbQueryResult(string name, string fieldName, ReadOnlyList<DbQueryResultField> fields)
        {
            Name = name;
            FieldName = fieldName;
            Fields = fields;
        }
    }
}