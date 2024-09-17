using System.Data;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Microsoft.Data.SqlClient;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

internal static class TableSchema
{
    public static GetTableSchemaResult GetTableSchema(IDbConnection connection, string? tableName)
    {
        SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder();

        DatabaseObjectMultipartName fourPartName = new DatabaseObjectMultipartName(connection.Database, tableName);
        string? owner = fourPartName.Schema;
        if (owner == null)
            owner = "dbo";

        string commandText = string.Format(@"declare @id int

select
    @id = o.object_id
from {0}.sys.objects o
join {0}.sys.schemas s
    on o.schema_id = s.schema_id
where
    s.name = {1}
    and o.name = {2}

select
    c.name as ColumnName,
    c.column_id as ColumnId,
    t.name as TypeName,
    c.is_nullable as IsNullable,
	c.is_computed as IsComputed,
    c.default_object_id as DefaultObjectId
from {0}.sys.columns c
join {0}.sys.types t
	on c.user_type_id = t.user_type_id
where c.object_id = @id
order by c.column_id

declare @index_id int
select top 1
    @index_id = i.index_id
from {0}.sys.indexes i (readpast)
cross apply
(
    select count(1) as [Count]
    from {0}.sys.index_columns ic (readpast)
    join {0}.sys.columns c (readpast)
        on ic.object_id = c.object_id
        and ic.column_id = c.column_id
    where
        ic.object_id = i.object_id
        and ic.index_id = i.index_id
        and c.is_identity = 1
) c
where
    i.object_id = @id
    and i.is_unique = 1
    and i.has_filter = 0
order by
    case when i.is_primary_key = 1 then 0 else 1 end,
    c.[Count],
    i.index_id

if @index_id is null
    select @index_id = i.index_id
    from  {0}.sys.indexes i (readpast)
    where
        i.object_id = @id
        and i.is_unique = 1

select ic.column_id as ColumnId
from {0}.sys.index_columns ic
where
    ic.object_id    = @id
    and ic.index_id = @index_id
order by ic.index_column_id",
            sqlCommandBuilder.QuoteIdentifier(fourPartName.Database),
            owner.ToNullableNVarChar(),
            fourPartName.Name.ToNullableNVarChar());

        IDbCommandExecutor executor = DbCommandExecutorFactory.Create(connection);
        GetTableSchemaResult getTableSchemaResult = null;
        executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
        {
            System.Collections.ObjectModel.ReadOnlyCollection<Column> columns = dataReader.ReadResult(128, ReadColumn).ToReadOnlyCollection();
            System.Collections.ObjectModel.ReadOnlyCollection<UniqueIndexColumn> uniqueIndexColumns = dataReader.ReadNextResult(128, ReadUniqueIndexColumn).ToReadOnlyCollection();
            getTableSchemaResult = new GetTableSchemaResult(columns, uniqueIndexColumns);
        });
        return getTableSchemaResult;
    }

    private static Column ReadColumn(IDataRecord dataRecord)
    {
        string columnName = dataRecord.GetString(0);
        int columnId = dataRecord.GetInt32(1);
        string typeName = dataRecord.GetString(2);
        bool? isNullable = dataRecord.GetNullableBoolean(3);
        bool isComputed = dataRecord.GetBoolean(4);
        int defaultObjectId = dataRecord.GetInt32(5);
        return new Column(columnName, columnId, typeName, isNullable, isComputed, defaultObjectId);
    }

    private static UniqueIndexColumn ReadUniqueIndexColumn(IDataRecord dataRecord)
    {
        int columnId = dataRecord.GetInt32(0);
        return new UniqueIndexColumn(columnId);
    }
}