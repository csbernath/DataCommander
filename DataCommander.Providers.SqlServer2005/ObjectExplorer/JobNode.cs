namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;
    using Foundation.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal sealed class JobNode : ITreeNode
    {
        private readonly JobCollectionNode jobs;
        private readonly string name;

        public JobNode(
            JobCollectionNode jobs,
            string name)
        {
            Contract.Requires<ArgumentNullException>(jobs != null);
            this.jobs = jobs;
            this.name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return this.name;
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                string commandText = $@"msdb..sp_help_job @job_name = {this.name.ToTSqlNVarChar()}";
                DataSet dataSet;
                using (var connection = new SqlConnection(this.jobs.Server.ConnectionString))
                {
                    var transactionScope = new DbTransactionScope(connection, null);
                    dataSet = transactionScope.ExecuteDataSet(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
                }

                var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                queryForm.ShowDataSet(dataSet);

                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}