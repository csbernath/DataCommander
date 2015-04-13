// -----------------------------------------------------------------------
// <copyright file="JobNode.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal sealed class JobNode : ITreeNode
    {
        private readonly JobCollectionNode jobs;
        private readonly string name;

        public JobNode( 
            JobCollectionNode jobs,
            string name )
        {
            Contract.Requires( jobs != null );
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

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
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
                string commandText = string.Format(@"msdb..sp_help_job @job_name = {0}", this.name.ToTSqlNVarChar());
                DataSet dataSet;
                using (var connection = new SqlConnection( this.jobs.Server.ConnectionString ))
                {
                    dataSet = connection.ExecuteDataSet( commandText );
                }

                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                queryForm.ShowDataSet( dataSet );

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
