using System;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class JobCollectionNode : ITreeNode
    {
        public JobCollectionNode(ServerNode server)
        {
            FoundationContract.Requires<ArgumentException>( server != null );

            Server = server;
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
            return SqlClientFactory.Instance.ExecuteReader(Server.ConnectionString, new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode) new JobNode(this, name);
            });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}