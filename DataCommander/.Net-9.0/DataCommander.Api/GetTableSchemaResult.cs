using System.Collections.ObjectModel;

namespace DataCommander.Api;

public class GetTableSchemaResult(ReadOnlyCollection<Column> columns, ReadOnlyCollection<UniqueIndexColumn> uniqueIndexColumns)
{
    public readonly ReadOnlyCollection<Column> Columns = columns;
    public readonly ReadOnlyCollection<UniqueIndexColumn> UniqueIndexColumns = uniqueIndexColumns;
}