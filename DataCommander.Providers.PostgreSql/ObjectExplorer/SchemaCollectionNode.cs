using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class SchemaCollectionNode : ITreeNode
    {
        public SchemaCollectionNode(ObjectExplorer objectExplorer) => ObjectExplorer = objectExplorer;

        public ObjectExplorer ObjectExplorer { get; }
        ContextMenuStrip ITreeNode.ContextMenu => null;
        bool ITreeNode.IsLeaf => false;
        string ITreeNode.Name => "Schemas";
        string ITreeNode.Query => null;
        bool ITreeNode.Sortable => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteReader(new ExecuteReaderRequest(@"select schema_name
from information_schema.schemata
order by schema_name"), dataReader =>
                {
                    dataReader.ReadResult(() =>
                    {
                        var name = dataReader.GetString(0);
                        var schemaNode = new SchemaNode(this, name);
                        nodes.Add(schemaNode);
                    });
                });

                return nodes;
            }
        }
    }
}