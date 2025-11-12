using System.Collections.Generic;

namespace DataCommander.Api;

public class GetTableSchemaResult(
    IReadOnlyCollection<Column> columns,
    IReadOnlyCollection<UniqueIndexColumn> uniqueIndexColumns)
{
    public readonly IReadOnlyCollection<Column> Columns = columns;
    public readonly IReadOnlyCollection<UniqueIndexColumn> UniqueIndexColumns = uniqueIndexColumns;
}