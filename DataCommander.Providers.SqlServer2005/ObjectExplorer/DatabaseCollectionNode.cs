namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        public DatabaseCollectionNode(ServerNode server)
        {
#if CONTRACTS_FULL
            Contract.Requires(server != null);
#endif
            this.Server = server;
        }

        public ServerNode Server { get; }

#region ITreeNode Members

        string ITreeNode.Name => "Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var list = new List<ITreeNode>();
            list.Add(new SystemDatabaseCollectionNode(this));

            var connectionString = this.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                const string commandText = @"select d.name
from sys.databases d (nolock)
where name not in('master','model','msdb','tempdb')
order by d.name";

                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);

                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var node = new DatabaseNode(this, name);
                        list.Add(node);
                    });
                }
            }

            return list;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}