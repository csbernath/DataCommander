﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Text;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class DatabaseNode : ITreeNode
    {
        private readonly string _name;
        private readonly byte _state;

        public DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string name, byte state)
        {
            Databases = databaseCollectionNode;
            _name = name;
            _state = state;
        }

        public DatabaseCollectionNode Databases { get; }

        public string Name => _name;

        string ITreeNode.Name
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(_name);

                if (_state == 6)
                    sb.Append(" (Offline)");

                return sb.ToString();
            }
        }

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

        public ContextMenu GetContextMenu()
        {
            var getInformationMenuItem = new MenuItem("Get information", GetInformationMenuItem_Click, EmptyReadOnlyCollection<MenuItem>.Value);
            var createDatabaseSnapshotMenuItem =
                new MenuItem("Create database snapshot script to clipboard", CreateDatabaseSnapshotScriptToClipboardMenuItem_Click, EmptyReadOnlyCollection<MenuItem>.Value);
            var menuItems = new[] { getInformationMenuItem, createDatabaseSnapshotMenuItem }.ToReadOnlyCollection();
            var contextMenu = new ContextMenu(menuItems);
            return contextMenu;
        }

        private void GetInformationMenuItem_Click(object sender, EventArgs e)
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
from	[{0}].sys.database_files f", _name);
            var connectionString = Databases.Server.ConnectionString;

            var queryForm = (IQueryForm)sender;
            DataSet dataSet = null;
            using (var connection = new SqlConnection(connectionString))
            {
                var executor = connection.CreateCommandExecutor();
                try
                {
                    dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText));
                }
                catch (SqlException sqlException)
                {
                    queryForm.ShowMessage(sqlException);
                }
            }

            if (dataSet != null) queryForm.ShowDataSet(dataSet);
        }
        
        private void CreateDatabaseSnapshotScriptToClipboardMenuItem_Click(object? sender, EventArgs e)
        {
            var databaseName = _name;

            var databaseSnapshotName = $"{databaseName}_Snapshot_{DateTime.Now.ToString("yyyyMMdd_HHmm")}";
            var logical_file_name = GetLogicalFileName(databaseName);
            var osFileName = $"D:\\Backup\\{databaseSnapshotName}.ss";

            var textBuilder = new TextBuilder();
            
            textBuilder.Add($"CREATE DATABASE [{databaseSnapshotName}]");
            textBuilder.Add("ON");
            
            using (textBuilder.AddBlock("(", ")"))
            {
                textBuilder.Add($"NAME = {logical_file_name},");
                textBuilder.Add($"FILENAME = {osFileName.ToVarChar()}");
            }

            textBuilder.Add($"AS SNAPSHOT OF [{databaseName}]");
            textBuilder.Add(Line.Empty);
            textBuilder.Add("USE master");
            textBuilder.Add($"ALTER DATABASE [{databaseName}] SET SINGLE_USER");
            textBuilder.Add($"RESTORE DATABASE [{databaseName}] FROM");
            textBuilder.Add($"DATABASE_SNAPSHOT = {databaseSnapshotName.ToVarChar()}");
            textBuilder.Add($"ALTER DATABASE [{databaseName}] SET MULTI_USER WITH NO_WAIT");

            var text = textBuilder.ToLines().ToIndentedString("  ");
            Clipboard.SetText(text);
        }

        private object GetLogicalFileName(string database)
        {
            string logicalFileName;
            
            var commandText = @$"select
    f.name
from [{database}].sys.database_files f
where
    f.type = 0";

            var connectionString = Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                var createCommandRequest = new CreateCommandRequest(commandText);
                var scalar = executor.ExecuteScalar(createCommandRequest);
                logicalFileName = (string)scalar;
            }

            return logicalFileName;
        }
    }
}