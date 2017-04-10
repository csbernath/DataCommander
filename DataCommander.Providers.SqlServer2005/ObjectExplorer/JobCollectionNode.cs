namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class JobCollectionNode : ITreeNode
    {
        public JobCollectionNode( ServerNode server )
        {
#if CONTRACTS_FULL
            Contract.Requires( server != null );
#endif
            this.Server = server;
        }

        public ServerNode Server { get; }

#region ITreeNode Members

        string ITreeNode.Name => "Jobs";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"select  j.name
from    msdb.dbo.sysjobs j (nolock)
order by j.name";
            using (var connection = new SqlConnection(this.Server.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    return dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        return (ITreeNode)new JobNode(this, name);
                    });
                }
            }
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}