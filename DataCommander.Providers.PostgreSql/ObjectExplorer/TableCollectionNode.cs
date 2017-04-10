namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Foundation.Data;
    using Npgsql;

    internal sealed class TableCollectionNode : ITreeNode
    {
        public TableCollectionNode(SchemaNode schemaNode)
        {
            this.SchemaNode = schemaNode;
        }

        public SchemaNode SchemaNode { get; }

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(this.SchemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition
                {
                    CommandText = $@"select table_name
from information_schema.tables
where
    table_schema = '{this.SchemaNode.Name}'
    and table_type = 'BASE TABLE'
order by table_name"
                }, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var schemaNode = new TableNode(this, name);
                        nodes.Add(schemaNode);
                        return true;
                    });
                }
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}