namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsGetExtendedItemsDataReader : TfsDataReader
    {
        private readonly TfsCommand command;
        private bool first = true;
        private ExtendedItem[] items;
        private int index;

        public TfsGetExtendedItemsDataReader(TfsCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            DataTable table = CreateSchemaTable();
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

            if (this.first)
            {
                this.first = false;
                TfsParameterCollection parameters = this.command.Parameters;
                string path = (string) parameters[ "path" ].Value;
                RecursionType recursion;
                TfsParameter parameter = parameters.FirstOrDefault( p => p.ParameterName == "recursion" );
                if (parameter != null)
                {
                    string recursionString = Database.GetValueOrDefault<string>( parameter.Value );
                    recursion = Enum<RecursionType>.Parse( recursionString );
                    
                }
                else
                {
                    recursion = RecursionType.Full;
                }

                VersionControlServer versionControlServer = this.command.Connection.VersionControlServer;
                Workspace[] workspaces = versionControlServer.QueryWorkspaces( null, null, Environment.MachineName );
                Workspace workspace = null;
                WorkingFolder workingFolder = null;

                foreach (Workspace currentWorkspace in workspaces)
                {
                    workingFolder = currentWorkspace.TryGetWorkingFolderForServerItem( path );

                    if (workingFolder != null)
                    {
                        workspace = currentWorkspace;
                        ItemSpec itemSpec = new ItemSpec( path, recursion );
                        ExtendedItem[][] extendedItems = currentWorkspace.GetExtendedItems( new ItemSpec[] { itemSpec }, DeletedState.Any, ItemType.Any );
                        this.items = extendedItems[ 0 ];
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

            if (this.items != null && this.index < this.items.Length)
            {
                ExtendedItem item = this.items[this.index ];

                object[] values = new object[]
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

                this.Values = values;
                read = true;
                this.index++;
            }
            else
            {
                read = false;
            }

            return read;
        }

        public override int RecordsAffected
        {
            get
            {
                return -1;
            }
        }

        public override int FieldCount
        {
            get
            {
                return 8;
            }
        }
    }
}