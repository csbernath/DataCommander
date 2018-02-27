using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Npgsql;

    internal sealed class SchemaCollectionNode : ITreeNode
    {
        public SchemaCollectionNode(ObjectExplorer objectExplorer)
        {
            ObjectExplorer = objectExplorer;
        }

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
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition
                {
                    CommandText = @"select schema_name
from information_schema.schemata
order by schema_name"
                }, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var schemaNode = new SchemaNode(this, name);
                        nodes.Add(schemaNode);
                        return true;
                    });
                }
            }

            return nodes;
        }
    }
}