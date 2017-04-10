namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class DatabaseNode : ITreeNode
    {
        public DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string name)
        {
            this.Databases = databaseCollectionNode;
            this.Name = name;
        }

        public DatabaseCollectionNode Databases { get; }

        public string Name { get; }

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var children = new ITreeNode[]
            {
                new TableCollectionNode(this),
                new ViewCollectionNode(this),
                new ProgrammabilityNode(this),
                new DatabaseSecurityNode(this)
            };

            return children;
        }

        public bool Sortable => false;

        string ITreeNode.Query => null;

        private void menuItemGetInformation_Click(object sender, EventArgs e)
        {
            var commandText = string.Format(@"select
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
from	[{0}].sys.database_files f", this.Name);
            var connectionString = this.Databases.Server.ConnectionString;
            var mainForm = DataCommanderApplication.Instance.MainForm;
            var queryForm = (QueryForm) mainForm.ActiveMdiChild;
            DataSet dataSet = null;
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                try
                {
                    dataSet = transactionScope.ExecuteDataSet(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
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
                var menuItemGetInformation = new ToolStripMenuItem("Get information", null,
                    new EventHandler(this.menuItemGetInformation_Click));
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemGetInformation);
                return contextMenu;
            }
        }
    }
}