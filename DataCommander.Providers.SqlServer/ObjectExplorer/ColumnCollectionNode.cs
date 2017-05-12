namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data;
    using Foundation.Diagnostics;
    using Foundation.Linq;

    internal sealed class ColumnCollectionNode : ITreeNode
    {
        private ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly DatabaseNode databaseNode;
        private readonly int id;

        public ColumnCollectionNode(DatabaseNode databaseNode, int id)
        {
            this.databaseNode = databaseNode;
            this.id = id;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Columns";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            SortedDictionary<int, ColumnNode> columnNodes;

            using (var methodLog = LogFactory.Instance.GetCurrentMethodLog())
            {
                using (var connection = new SqlConnection(this.databaseNode.Databases.Server.ConnectionString))
                {
                    connection.Open();
                    var cb = new SqlCommandBuilder();
                    var databaseName = cb.QuoteIdentifier(this.databaseNode.Name);
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

                    methodLog.Write(LogLevel.Trace, "commandText:\r\n{0}", commandText);
                    var transactionScope = new DbTransactionScope(connection, null);
                    using (var reader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                    {
                        columnNodes = reader.Read(dataRecord => ToColumnNode(dataRecord)).ToSortedDictionary(c => c.Id);

                        reader.Read(dataRecord =>
                        {
                            var columnId = dataRecord.GetInt32(0);
                            var columnNode = columnNodes[columnId];
                            columnNode.IsPrimaryKey = true;
                        });

                        reader.Read(dataRecord =>
                        {
                            var columnId = dataRecord.GetInt32(0);
                            var columnNode = columnNodes[columnId];
                            columnNode.IsForeignKey = true;
                        });
                    }
                }
            }

            return columnNodes.Values;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion

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
    }
}