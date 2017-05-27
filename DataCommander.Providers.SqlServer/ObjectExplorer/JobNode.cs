using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Query;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal sealed class JobNode : ITreeNode
    {
        private readonly JobCollectionNode _jobs;
        private readonly string _name;

        public JobNode(
            JobCollectionNode jobs,
            string name)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(jobs != null);
#endif
            _jobs = jobs;
            _name = name;
        }

#region ITreeNode Members

        string ITreeNode.Name => _name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query
        {
            get
            {
                var commandText = $@"msdb..sp_help_job @job_name = {_name.ToTSqlNVarChar()}";
                DataSet dataSet;
                using (var connection = new SqlConnection(_jobs.Server.ConnectionString))
                {
                    var transactionScope = new DbTransactionScope(connection, null);
                    dataSet = transactionScope.ExecuteDataSet(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
                }

                var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                queryForm.ShowDataSet(dataSet);

                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}