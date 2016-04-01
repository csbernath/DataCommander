namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation;

    internal sealed class ServerNode : ITreeNode
    {
        public ServerNode(string connectionString)
        {
            Contract.Requires(!connectionString.IsNullOrWhiteSpace());
            this.ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                string serverVersion;
                using (var connection = new SqlConnection(this.ConnectionString))
                {
                    connection.Open();
                    serverVersion = new Version(connection.ServerVersion).ToString();
                }

                var csb = new SqlConnectionStringBuilder(this.ConnectionString);
                string userName;
                if (csb.IntegratedSecurity)
                {
                    userName = Environment.UserDomainName + "\\" + Environment.UserName;
                }
                else
                {
                    userName = csb.UserID;
                }

                return $"{csb.DataSource}(SQL Server {serverVersion} - {userName})";
            }
        }

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var node = new DatabaseCollectionNode(this);
            var securityNode = new SecurityNode(this);
            var serverObjectCollectionNode = new ServerObjectCollectionNode(this);
            var jobCollectionNode = new JobCollectionNode(this);
            return new ITreeNode[] {node, securityNode, serverObjectCollectionNode, jobCollectionNode};
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}