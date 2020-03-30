using Foundation.Collections.ReadOnly;

namespace DataCommander.Providers2
{
    public class GetTableSchemaResult
    {
        public readonly ReadOnlyList<Column> Columns;
        public readonly ReadOnlyList<UniqueIndexColumn> UniqueIndexColumns;

        public GetTableSchemaResult(ReadOnlyList<Column> columns, ReadOnlyList<UniqueIndexColumn> uniqueIndexColumns)
        {
            Columns = columns;
            UniqueIndexColumns = uniqueIndexColumns;
        }
    }
}