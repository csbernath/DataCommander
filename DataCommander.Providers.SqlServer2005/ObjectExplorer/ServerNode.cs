namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;

    internal sealed class ServerNode : ITreeNode
    {
        private readonly string connectionString;

        public ServerNode(string connectionString)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(connectionString));
            this.connectionString = connectionString;
        }

        public string ConnectionString => this.connectionString;

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                string serverVersion;
                using (var connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    serverVersion = new Version(connection.ServerVersion).ToString();
                }

                var csb = new SqlConnectionStringBuilder(this.connectionString);
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