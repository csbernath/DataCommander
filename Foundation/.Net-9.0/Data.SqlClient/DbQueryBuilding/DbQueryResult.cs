using System.Collections.ObjectModel;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DbQueryResult(string name, string fieldName, ReadOnlyCollection<DbQueryResultField> fields)
{
    public readonly string Name = name;
    public readonly string FieldName = fieldName;
    public readonly ReadOnlyCollection<DbQueryResultField> Fields = fields;
}