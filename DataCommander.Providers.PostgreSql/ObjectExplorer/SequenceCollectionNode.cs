using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Npgsql;

    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;

        public SequenceCollectionNode(SchemaNode schemaNode)
        {
            this.schemaNode = schemaNode;
        }

        string ITreeNode.Name => "Sequences";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(this.schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition
                {
                    CommandText = $@"select sequence_name
from information_schema.sequences
where sequence_schema = '{this.schemaNode.Name}'
order by sequence_name"
                }, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var schemaNode = new SequenceNode(this, name);
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