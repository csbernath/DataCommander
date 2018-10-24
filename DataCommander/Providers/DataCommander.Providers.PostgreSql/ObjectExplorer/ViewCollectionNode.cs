using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class ViewCollectionNode : ITreeNode
    {
        private readonly SchemaNode _schemaNode;

        public ViewCollectionNode(SchemaNode schemaNode)
        {
            _schemaNode = schemaNode;
        }

        string ITreeNode.Name => "Views";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(_schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteReader(new ExecuteReaderRequest($@"select table_name
from information_schema.views
where table_schema = '{_schemaNode.Name}'
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