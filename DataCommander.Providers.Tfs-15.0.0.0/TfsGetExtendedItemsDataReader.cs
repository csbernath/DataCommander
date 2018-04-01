using Foundation;
using Foundation.Assertions;
using Foundation.Data;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using System.Linq;
    using Connection;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Query;

    internal sealed class TfsGetExtendedItemsDataReader : TfsDataReader
    {
        private readonly TfsCommand command;
        private bool first = true;
        private ExtendedItem[] items;
        private int index;

        public TfsGetExtendedItemsDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
            AddSchemaRowString( table, "SourceServerItem", false );
            AddSchemaRowString( table, "ChangeType", false );
            AddSchemaRowString( table, "LockOwner", false );
            AddSchemaRowString( table, "LockStatus", false );
            AddSchemaRowBoolean( table, "IsLatest", false );
            AddSchemaRowBoolean( table, "HasOtherPendingChange", false );
            AddSchemaRowInt32( table, "VersionLatest", false );
            AddSchemaRowInt32( table, "VersionLocal", false );
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (first)
            {
                first = false;
                var parameters = command.Parameters;
                var path = (string) parameters[ "path" ].Value;
                RecursionType recursion;
                var parameter = parameters.FirstOrDefault( p => p.ParameterName == "recursion" );
                if (parameter != null)
                {
                    var recursionString = ValueReader.GetValueOrDefault<string>( parameter.Value );
                    recursion = Enum<RecursionType>.Parse( recursionString );
                    
                }
                else
                {
                    recursion = RecursionType.Full;
                }

                var versionControlServer = command.Connection.VersionControlServer;
                var workspaces = versionControlServer.QueryWorkspaces( null, null, Environment.MachineName );
                Workspace workspace = null;
                WorkingFolder workingFolder = null;

                foreach (var currentWorkspace in workspaces)
                {
                    workingFolder = currentWorkspace.TryGetWorkingFolderForServerItem( path );

                    if (workingFolder != null)
                    {
                        workspace = currentWorkspace;
                        var itemSpec = new ItemSpec( path, recursion );
                        var extendedItems = currentWorkspace.GetExtendedItems( new ItemSpec[] { itemSpec }, DeletedState.Any, ItemType.Any );
                        items = extendedItems[ 0 ];
                    }
                }

                if (workingFolder == null)
                {
                    throw new Exception($"Workspace not found for '{path}'");
                }

                string name;

                if (workspace != null)
                {
                    name = workspace.Name;
                }
                else
                {
                    name = "(not found)";
                }

                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                queryForm.AddInfoMessage( new InfoMessage( LocalTime.Default.Now, InfoMessageSeverity.Information,
                    $"\r\nworkspace.Name: {name}\r\nworkingFolder.LocalItem: {workingFolder.LocalItem}") );
            }

            if (items != null && index < items.Length)
            {
                var item = items[index ];

                var values = new object[]
                {
                    item.SourceServerItem,
                    item.ChangeType.ToString(),
                    item.LockOwner,
                    item.LockStatus.ToString(),
                    item.IsLatest,
                    item.HasOtherPendingChange,
                    item.VersionLatest,
                    item.VersionLocal
                };

                Values = values;
                read = true;
                index++;
            }
            else
            {
                read = false;
            }

            return read;
        }

        public override int RecordsAffected => -1;

        public override int FieldCount => 8;
    }
}