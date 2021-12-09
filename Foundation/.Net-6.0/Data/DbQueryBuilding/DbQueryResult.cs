using System.Collections.ObjectModel;

namespace Foundation.Data.DbQueryBuilding;

public sealed class DbQueryResult
{
    public readonly string Name;
    public readonly string FieldName;
    public readonly ReadOnlyCollection<DbQueryResultField> Fields;

    public DbQueryResult(string name, string fieldName, ReadOnlyCollection<DbQueryResultField> fields)
    {
        Name = name;
        FieldName = fieldName;
        Fields = fields;

    }
}