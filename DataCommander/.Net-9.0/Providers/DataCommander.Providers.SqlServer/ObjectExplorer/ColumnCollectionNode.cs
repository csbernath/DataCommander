using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ColumnCollectionNode(DatabaseNode databaseNode, int id) : ITreeNode
{
    private readonly ILog _log = LogFactory.Instance.GetCurrentTypeLog();

    private static ColumnNode ToColumnNode(IDataRecord dataRecord)
    {
        int id = dataRecord.GetInt32(0);
        string columnName = dataRecord.GetString(1);
        byte systemTypeId = dataRecord.GetByte(2);
        short maxLength = dataRecord.GetInt16(3);
        byte precision = dataRecord.GetByte(4);
        byte scale = dataRecord.GetByte(5);
        bool isNullable = dataRecord.GetBoolean(6);
        bool isComputed = dataRecord.GetBoolean(7);
        string userTypeName = dataRecord.GetString(8);

        return new ColumnNode(id, columnName, systemTypeId, maxLength, precision, scale, isNullable, isComputed, userTypeName);
    }

    string? ITreeNode.Name => "Columns";

    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        string commandText = CreateCommandText();
        SortedDictionary<int, ColumnNode> columnNodes = null;
        await Db.ExecuteReaderAsync(
            databaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            async (dataReader, _) =>
            {
                columnNodes = (await dataReader.ReadResultAsync(128, ToColumnNode, cancellationToken))
                    .ToSortedDictionary(c => c.Id);
                await dataReader.NextResultAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    int columnId = dataReader.GetInt32(0);
                    ColumnNode columnNode = columnNodes[columnId];
                    columnNode.IsPrimaryKey = true;
                }

                await dataReader.NextResultAsync(cancellationToken);
                while (await dataReader.ReadAsync(cancellationToken))
                {
                    int columnId = dataReader.GetInt32(0);
                    ColumnNode columnNode = columnNodes[columnId];
                    columnNode.IsForeignKey = true;
                }
            },
            cancellationToken);
        return columnNodes.Values;
    }

    private string CreateCommandText()
    {
        SqlCommandBuilder cb = new SqlCommandBuilder();
        string databaseName = cb.QuoteIdentifier(databaseNode.Name);
        string commandText = $@"select
    c.column_id,
    c.name,
    c.system_type_id,
    c.max_length,
    c.precision,
    c.scale,
    c.is_nullable,
    c.is_computed,
    t.name as UserTypeName
from    {databaseName}.sys.all_columns c
join    {databaseName}.sys.types t
on      c.user_type_id = t.user_type_id
where   c.object_id = {id}
order by c.column_id

declare @index_id int

select  @index_id = i.index_id
from    {databaseName}.sys.indexes i
where   i.object_id = {id}
        and i.is_primary_key = 1

select  ic.column_id
from    {databaseName}.sys.index_columns ic
where   ic.object_id = {id}
        and ic.index_id = @index_id

select  fkc.parent_column_id
from    {databaseName}.sys.foreign_key_columns fkc
where   fkc.parent_object_id = {id}
order by fkc.parent_column_id";
        return commandText;
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}