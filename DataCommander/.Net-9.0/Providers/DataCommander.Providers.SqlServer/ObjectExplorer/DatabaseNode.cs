using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Text;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string? name, byte state) : ITreeNode
{
    public DatabaseCollectionNode Databases { get; } = databaseCollectionNode;

    public string? Name => name;

    string? ITreeNode.Name
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name);

            if (state == 6)
                sb.Append(" (Offline)");

            return sb.ToString();
        }
    }

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] children = new ITreeNode[]
        {
            new TableCollectionNode(this),
            new ViewCollectionNode(this),
            new ProgrammabilityNode(this),
            new DatabaseSecurityNode(this)
        };

        return Task.FromResult<IEnumerable<ITreeNode>>(children);
    }

    public bool Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        MenuItem getInformationMenuItem = new MenuItem("Get information", GetInformationMenuItem_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        MenuItem createDatabaseSnapshotMenuItem =
            new MenuItem("Create database snapshot script to clipboard", CreateDatabaseSnapshotScriptToClipboardMenuItem_Click, EmptyReadOnlyCollection<MenuItem>.Value);
        System.Collections.ObjectModel.ReadOnlyCollection<MenuItem> menuItems = new[] { getInformationMenuItem, createDatabaseSnapshotMenuItem }.ToReadOnlyCollection();
        ContextMenu contextMenu = new ContextMenu(menuItems);
        return contextMenu;
    }

    private void GetInformationMenuItem_Click(object sender, EventArgs e)
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
	convert(decimal(15),f.size * 8096.0 / 1000000000)						as [Total Size (GB)],
	convert(decimal(15),fileproperty(f.name, 'SpaceUsed') * 8096.0 / 1000000000)		as [Used (GB)],
	convert(decimal(15,2),convert(float,fileproperty(name, 'SpaceUsed')) * 100.0 / size)	as [Used%],
	convert(decimal(15,2),(f.size-fileproperty(name, 'SpaceUsed')) * 8096.0 / 1000000000)	as [Free (GB)]
from	[{0}].sys.database_files f", name);
        IQueryForm queryForm = (IQueryForm)sender;
        DataSet dataSet = null;
        using (SqlConnection connection = Databases.Server.CreateConnection())
        {
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            try
            {
                dataSet = executor.ExecuteDataSet(new ExecuteReaderRequest(commandText), CancellationToken.None);
            }
            catch (SqlException sqlException)
            {
                queryForm.ShowMessage(sqlException);
            }
        }

        if (dataSet != null)
            queryForm.ShowDataSet(dataSet);
    }

    private void CreateDatabaseSnapshotScriptToClipboardMenuItem_Click(object? sender, EventArgs e)
    {
        string? databaseName = name;

        string databaseSnapshotName = $"{databaseName}_Snapshot_{DateTime.Now:yyyyMMdd_HHmm}";
        string logical_file_name = GetLogicalFileName(databaseName);
        string osFileName = $"D:\\Backup\\{databaseSnapshotName}.ss";

        TextBuilder textBuilder = new TextBuilder();

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

        string text = textBuilder.ToLines().ToIndentedString("  ");
        IQueryForm? queryForm = (IQueryForm)sender;
        queryForm.SetClipboardText(text);
    }

    private string GetLogicalFileName(string? database)
    {
        string logicalFileName;

        string commandText = @$"select
    f.name
from [{database}].sys.database_files f
where
    f.type = 0";
        using (SqlConnection connection = Databases.Server.CreateConnection())
        {
            connection.Open();
            IDbCommandExecutor executor = connection.CreateCommandExecutor();
            CreateCommandRequest createCommandRequest = new CreateCommandRequest(commandText);
            object scalar = executor.ExecuteScalar(createCommandRequest);
            logicalFileName = (string)scalar;
        }

        return logicalFileName;
    }
}