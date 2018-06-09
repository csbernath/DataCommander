using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Npgsql;

    internal sealed class ColumnCollectionNode : ITreeNode
    {
        private readonly TableNode _tableNode;

        public ColumnCollectionNode(TableNode tableNode)
        {
            this._tableNode = tableNode;
        }

        string ITreeNode.Name => "Columns";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();
            var schemaNode = _tableNode.TableCollectionNode.SchemaNode;

            using (var connection = new NpgsqlConnection(schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteReader(new ExecuteReaderRequest($@"select
     c.column_name
    ,c.is_nullable
    ,c.data_type
    ,c.character_maximum_length
    ,c.numeric_precision
    ,c.numeric_scale
from information_schema.columns c
where
    c.table_schema = '{_tableNode.TableCollectionNode.SchemaNode.Name}'
    and c.table_name = '{_tableNode.Name}'
order by c.ordinal_position"), dataRecord =>
                {
                    var columnName = dataRecord.GetString(0);
                    var dataType = dataRecord.GetString(2);
                    var columnNode = new ColumnNode(this, columnName, dataType);
                    nodes.Add(columnNode);
                });
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}