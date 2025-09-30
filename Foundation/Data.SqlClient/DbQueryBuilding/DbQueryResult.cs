using System.Collections.Generic;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DbQueryResult(string name, string fieldName, IReadOnlyCollection<DbQueryResultField> fields)
{
    public readonly string Name = name;
    public readonly string FieldName = fieldName;
    public readonly IReadOnlyCollection<DbQueryResultField> Fields = fields;
}