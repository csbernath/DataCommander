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

            using (var connection = new NpgsqlConnection(schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteReader(new ExecuteReaderRequest($@"select table_name
from information_schema.views
where table_schema = '{schemaNode.Name}'
order by table_name"), dataReader => dataReader.ReadResult(
                    () =>
                    {
                        var name = dataReader.GetString(0);
                        var viewNode = new ViewNode(this, name);
                        nodes.Add(viewNode);
                    }));
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}