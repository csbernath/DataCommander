using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Collections.ReadOnly;
using Foundation.Data;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ServerNode : ITreeNode
{
    public readonly ConnectionStringAndCredential ConnectionStringAndCredential;
    
    public ServerNode(ConnectionStringAndCredential connectionStringAndCredential)
    {
        ConnectionStringAndCredential = connectionStringAndCredential;
    }

    public SqlConnection CreateConnection() => ConnectionFactory.CreateConnection(ConnectionStringAndCredential);

    string ITreeNode.Name => ConnectionNameProvider.GetConnectionName(CreateConnection);

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var node = new DatabaseCollectionNode(this);
        var securityNode = new SecurityNode(this);
        var serverObjectCollectionNode = new ServerObjectCollectionNode(this);
        var jobCollectionNode = new JobCollectionNode(this);
        return Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[] { node, securityNode, serverObjectCollectionNode, jobCollectionNode });
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu()
    {
        var menuItems = new MenuItem[]
        {
            new("Properties", Properties_OnClick, EmptyReadOnlyCollection<MenuItem>.Value)
        }.ToReadOnlyCollection();
        return new ContextMenu(menuItems);
    }

    private void Properties_OnClick(object? sender, EventArgs e)
    {
        var commandText = @"create table #SVer(ID int,  Name  sysname, Internal_Value int, Value nvarchar(512))
insert #SVer exec master.dbo.xp_msver
insert #SVer select t.*
from sys.dm_os_host_info
CROSS APPLY (
VALUES
(1001, 'host_platform', 0, host_platform),
(1002, 'host_distribution', 0, host_distribution),
(1003, 'host_release', 0, host_release),
(1004, 'host_service_pack_level', 0, host_service_pack_level),
(1005, 'host_sku', host_sku, ''),
(1006, 'HardwareGeneration', '', ''),
(1007, 'ServiceTier', '', ''),
(1008, 'ReservedStorageSizeMB', '0', '0'),
(1009, 'UsedStorageSizeMB', '0', '0')
) t(id, [name], internal_value, [value])

-- Managed Instance-specific properties
if (SERVERPROPERTY('EngineEdition') = 8)
begin
DECLARE @gen4memoryPerCoreMB float = 7168.0
DECLARE @gen5memoryPerCoreMB float = 5223.0
DECLARE @physicalMemory float
DECLARE @virtual_core_count int
DECLARE @reservedStorageSize bigint
DECLARE @usedStorageSize decimal(18,2)
DECLARE @hwGeneration nvarchar(128)
DECLARE @serviceTier nvarchar(128)

SET @physicalMemory = (SELECT TOP 1 [virtual_core_count] *
  (
	CASE WHEN [hardware_generation] = 'Gen4' THEN @gen4memoryPerCoreMB
	WHEN [hardware_generation] = 'Gen5' THEN @gen5memoryPerCoreMB
	ELSE 0 END
   )
   FROM master.sys.server_resource_stats 
   ORDER BY start_time DESC)

IF (@physicalMemory <> 0) 
BEGIN
  UPDATE #SVer SET [Internal_Value] =  @physicalMemory WHERE Name = N'PhysicalMemory'
  UPDATE #SVer SET [Value] = CONCAT( @physicalMemory, ' (',  @physicalMemory * 1024, ')') WHERE Name = N'PhysicalMemory'
END

UPDATE #SVer SET [Internal_Value] = (SELECT TOP 1 [virtual_core_count] FROM master.sys.server_resource_stats ORDER BY start_time desc) WHERE Name = N'ProcessorCount'
UPDATE #SVer SET [Value] = [Internal_Value] WHERE Name = N'ProcessorCount'

SELECT TOP 1
  @hwGeneration = [hardware_generation],
  @serviceTier =[sku],
  @virtual_core_count = [virtual_core_count],
  @reservedStorageSize = [reserved_storage_mb],
  @usedStorageSize = [storage_space_used_mb]
FROM master.sys.server_resource_stats
ORDER BY [start_time] DESC

UPDATE #SVer SET [Value] = @hwGeneration WHERE Name = N'HardwareGeneration'
UPDATE #SVer SET [Value] = @serviceTier WHERE Name = N'ServiceTier'
UPDATE #SVer SET [Value] = @reservedStorageSize WHERE Name = N'ReservedStorageSizeMB'
UPDATE #SVer SET [Value] = @usedStorageSize WHERE Name = N'UsedStorageSizeMB'
end



declare @SmoRoot nvarchar(512)
exec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'SOFTWARE\Microsoft\MSSQLServer\Setup', N'SQLPath', @SmoRoot OUTPUT



SELECT
(select Value from #SVer where Name = N'ProductName') AS [Product],
SERVERPROPERTY(N'ProductVersion') AS [VersionString],
(select Value from #SVer where Name = N'Language') AS [Language],
(select Value from #SVer where Name = N'Platform') AS [Platform],
CAST(SERVERPROPERTY(N'Edition') AS sysname) AS [Edition],
(select Internal_Value from #SVer where Name = N'ProcessorCount') AS [Processors],
(select Value from #SVer where Name = N'WindowsVersion') AS [OSVersion],
(select Internal_Value from #SVer where Name = N'PhysicalMemory') AS [PhysicalMemory],
CAST(ISNULL(SERVERPROPERTY('IsClustered'),N'') AS bit) AS [IsClustered],
@SmoRoot AS [RootDirectory],
convert(sysname, serverproperty(N'collation')) AS [Collation],
( select Value from #SVer where Name =N'host_platform') AS [HostPlatform],
( select Value from #SVer where Name =N'host_release') AS [HostRelease],
( select Value from #SVer where Name =N'host_service_pack_level') AS [HostServicePackLevel],
( select Value from #SVer where Name =N'host_distribution') AS [HostDistribution]

drop table #SVer";


        var dataTable = new DataTable();
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Value");
        
        Db.ExecuteReader(CreateConnection, new ExecuteReaderRequest(commandText), dataReader =>
        {
            while (dataReader.Read())
            {
                for (var index = 0; index < dataReader.FieldCount; ++index)
                {
                    var name = dataReader.GetName(index);
                    var value = dataReader.GetValue(index);
                    dataTable.Rows.Add([name, value]);
                }
            }
        });

        var dataSet = new DataSet();
        dataSet.Tables.Add(dataTable);
        
        var queryForm = (IQueryForm)sender;
        queryForm.ShowDataSet(dataSet);
    }
}