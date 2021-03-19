using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer2.ObjectExplorer
{
    internal sealed class JobCollectionNode : ITreeNode
    {
        public JobCollectionNode(ServerNode server)
        {
            Assert.IsNotNull(server);
            Server = server;
        }

        public ServerNode Server { get; }

        string ITreeNode.Name => "Jobs";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"select  j.name
from    msdb.dbo.sysjobs j (nolock)
order by j.name";
            return SqlClientFactory.Instance.ExecuteReader(Server.ConnectionString, new ExecuteReaderRequest(commandText), 128, dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode) new JobNode(this, name);
            });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}