namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Application = DataCommander.Providers.Application;

    internal sealed class DatabaseNode : ITreeNode
    {
        private readonly DatabaseCollectionNode databaseCollectionNode;
        private readonly string name;

        public DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string name)
        {
            this.databaseCollectionNode = databaseCollectionNode;
            this.name = name;
        }

        public DatabaseCollectionNode Databases
        {
            get
            {
                return this.databaseCollectionNode;
            }
        }

        public string Name
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
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            ITreeNode[] children = new ITreeNode[]
            {
              new TableCollectionNode(this),
              new ViewCollectionNode(this),
              new ProgrammabilityNode(this),
              new UserCollectionNode(this),
              new RoleCollectionNode(this)
            };

            return children;
        }

        public bool Sortable
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
                return null;
            }
        }

        private void menuItemGetInformation_Click(object sender, EventArgs e)
        {
            string commandText = string.Format(@"select
    d.dbid,
    d.filename,
    DATABASEPROPERTYEX('{0}','Collation')         as [Collation],
    DATABASEPROPERTYEX('{0}','IsFulltextEnabled') as [IsFulltextEnabled],
    DATABASEPROPERTYEX('{0}','Recovery')          as Recovery,
    DATABASEPROPERTYEX('{0}','Status')            as Status
from master.dbo.sysdatabases d
where name = '{0}'

use [{0}]

select
    f.name,
    f.physical_name,
	convert(decimal(15,4),f.size * 8096.0 / 1000000)						as [Total Size (MB)],
	convert(decimal(15,4),fileproperty(f.name, 'SpaceUsed') * 8096.0 / 1000000)		as [Used (MB)],
	convert(decimal(15,2),convert(float,fileproperty(name, 'SpaceUsed')) * 100.0 / size)	as [Used%],
	convert(decimal(15,4),(f.size-fileproperty(name, 'SpaceUsed')) * 8096.0 / 1000000)	as [Free (MB)]
from	[{0}].sys.database_files f", this.name);
            string connectionString = this.databaseCollectionNode.Server.ConnectionString;
            MainForm mainForm = Application.Instance.MainForm;
            QueryForm queryForm = (QueryForm)mainForm.ActiveMdiChild;
            DataSet dataSet = null;
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    dataSet = connection.ExecuteDataSet(commandText);
                }
                catch (SqlException sqlException)
                {
                    queryForm.ShowMessage(sqlException);
                }
            }
            if (dataSet != null)
            {
                queryForm.ShowDataSet(dataSet);
            }
        }    

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                ToolStripMenuItem menuItemGetInformation = new ToolStripMenuItem("Get information", null, new EventHandler(this.menuItemGetInformation_Click));
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemGetInformation);
                return contextMenu;
            }
        }
    }
}