using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Npgsql;

    internal sealed class ColumnCollectionNode : ITreeNode
    {
        private readonly TableNode tableNode;

        public ColumnCollectionNode(TableNode tableNode)
        {
            this.tableNode = tableNode;
        }

        string ITreeNode.Name => "Columns";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();
            var schemaNode = this.tableNode.TableCollectionNode.SchemaNode;

            using (var connection = new NpgsqlConnection(schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                transactionScope.ExecuteReader(new CommandDefinition
                {
                    CommandText =
                        $@"select
     c.column_name
    ,c.is_nullable
    ,c.data_type
    ,c.character_maximum_length
    ,c.numeric_precision
    ,c.numeric_scale
from information_schema.columns c
where
    c.table_schema = '{this
                            .tableNode.TableCollectionNode.SchemaNode.Name}'
    and c.table_name = '{this.tableNode.Name}'
order by c.ordinal_position"
                }, CommandBehavior.Default, dataRecord =>
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