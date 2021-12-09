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
            using (var connection = new NpgsqlConnection(_schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                return executor.ExecuteReader(new ExecuteReaderRequest($@"select table_name
from information_schema.views
where table_schema = '{_schemaNode.Name}'
order by table_name"), 128, dataReader =>
                {
                    var name = dataReader.GetString(0);
                    return new ViewNode(this, name);
                });
            }
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}