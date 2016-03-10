namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Foundation.Data;
    using Npgsql;

    internal sealed class SchemaCollectionNode : ITreeNode
    {
        private readonly ObjectExplorer objectExplorer;

        public SchemaCollectionNode(ObjectExplorer objectExplorer)
        {
            this.objectExplorer = objectExplorer;
        }

        public ObjectExplorer ObjectExplorer
        {
            get
            {
                return this.objectExplorer;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Name
        {
            get
            {
                return "Schemas";
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(this.objectExplorer.ConnectionString))
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
                        string name = dataRecord.GetString(0);
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