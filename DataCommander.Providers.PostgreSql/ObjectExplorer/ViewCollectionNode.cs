using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Npgsql;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;

        public ViewCollectionNode(SchemaNode schemaNode)
        {
            this.schemaNode = schemaNode;
        }

        string ITreeNode.Name => "Views";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(this.schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                transactionScope.ExecuteReader(new CommandDefinition
                {
                    CommandText = $@"select table_name
from information_schema.views
where table_schema = '{this.schemaNode.Name}'
order by table_name"
                }, CommandBehavior.Default, dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    var viewNode = new ViewNode(this, name);
                    nodes.Add(viewNode);
                });
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}