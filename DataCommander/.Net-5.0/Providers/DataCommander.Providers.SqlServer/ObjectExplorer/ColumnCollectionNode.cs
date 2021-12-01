using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Data;
using Foundation.Linq;
using Foundation.Log;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class ColumnCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;
        private readonly int _id;
        private ILog _log = LogFactory.Instance.GetCurrentTypeLog();

        public ColumnCollectionNode(DatabaseNode databaseNode, int id)
        {
            _databaseNode = databaseNode;
            _id = id;
        }

        #region Private Methods

        private static ColumnNode ToColumnNode(IDataRecord dataRecord)
        {
            var id = dataRecord.GetInt32(0);
            var columnName = dataRecord.GetString(1);
            var systemTypeId = dataRecord.GetByte(2);
            var maxLength = dataRecord.GetInt16(3);
            var precision = dataRecord.GetByte(4);
            var scale = dataRecord.GetByte(5);
            var isNullable = dataRecord.GetBoolean(6);
            var userTypeName = dataRecord.GetString(7);

            return new ColumnNode(id, columnName, systemTypeId, maxLength, precision, scale, isNullable, userTypeName);
        }

        #endregion

        #region ITreeNode Members

        string ITreeNode.Name => "Columns";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            SortedDictionary<int, ColumnNode> columnNodes = null;

            using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
            {
                using (var connection = new SqlConnection(_databaseNode.Databases.Server.ConnectionString))
                {
                    connection.Open();
                    var cb = new SqlCommandBuilder();
                    var databaseName = cb.QuoteIdentifier(_databaseNode.Name);
                    var commandText = $@"select
     c.column_id
    ,c.name
    ,c.system_type_id
    ,c.max_length
    ,c.precision
    ,c.scale
    ,c.is_nullable
    ,t.name as UserTypeName
from    {databaseName}.sys.all_columns c
join    {databaseName}.sys.types t
on      c.user_type_id = t.user_type_id
where   c.object_id = {_id}
order by c.column_id

declare @index_id int

select  @index_id = i.index_id
from    {databaseName}.sys.indexes i
where   i.object_id = {_id}
        and i.is_primary_key = 1

select  ic.column_id
from    {databaseName}.sys.index_columns ic
where   ic.object_id = {_id}
        and ic.index_id = @index_id

select  fkc.parent_column_id
from    {databaseName}.sys.foreign_key_columns fkc
where   fkc.parent_object_id = {_id}
order by fkc.parent_column_id";

                    methodLog.Write(LogLevel.Trace, "commandText:\r\n{0}", commandText);
                    var executor = DbCommandExecutorFactory.Create(connection);
                    executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
                    {
                        columnNodes = dataReader.ReadResult(128, ToColumnNode).ToSortedDictionary(c => c.Id);
                        dataReader.NextResult();
                        while (dataReader.Read())
                        {
                            var columnId = dataReader.GetInt32(0);
                            var columnNode = columnNodes[columnId];
                            columnNode.IsPrimaryKey = true;
                        }

                        dataReader.NextResult();
                        while (dataReader.Read())
                        {
                            var columnId = dataReader.GetInt32(0);
                            var columnNode = columnNodes[columnId];
                            columnNode.IsForeignKey = true;
                        }
                    });
                }
            }

            return columnNodes.Values;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => throw new NotSupportedException();
        public ContextMenu GetContextMenu() => null;

        #endregion
    }
}