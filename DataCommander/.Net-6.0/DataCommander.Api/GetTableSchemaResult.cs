﻿using System.Collections.ObjectModel;

namespace DataCommander.Api;

public class GetTableSchemaResult
{
    public readonly ReadOnlyCollection<Column> Columns;
    public readonly ReadOnlyCollection<UniqueIndexColumn> UniqueIndexColumns;

    public GetTableSchemaResult(ReadOnlyCollection<Column> columns, ReadOnlyCollection<UniqueIndexColumn> uniqueIndexColumns)
    {
        Columns = columns;
        UniqueIndexColumns = uniqueIndexColumns;
    }
}