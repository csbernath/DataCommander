using System;
using System.Data;
using System.Linq;
using DataCommander.Providers.Connection;
using DataCommander.Providers.Query;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsGetExtendedItemsDataReader : TfsDataReader
    {
        private readonly TfsCommand _command;
        private bool _first = true;
        private ExtendedItem[] _items;
        private int _index;

        public TfsGetExtendedItemsDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            _command = command;
        }

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
            AddSchemaRowString(table, "SourceServerItem", false);
            AddSchemaRowString(table, "ChangeType", false);
            AddSchemaRowString(table, "LockOwner", false);
            AddSchemaRowString(table, "LockStatus", false);
            AddSchemaRowBoolean(table, "IsLatest", false);
            AddSchemaRowBoolean(table, "HasOtherPendingChange", false);
            AddSchemaRowInt32(table, "VersionLatest", false);
            AddSchemaRowInt32(table, "VersionLocal", false);
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (_first)
            {
                _first = false;
                var parameters = _command.Parameters;
                var path = (string) parameters["path"].Value;
                RecursionType recursion;
                var parameter = parameters.FirstOrDefault(p => p.ParameterName == "recursion");
                if (parameter != null)
                {
                    var recursionString = ValueReader.GetValueOrDefault<string>(parameter.Value);
                    recursion = Enum<RecursionType>.Parse(recursionString);

                }
                else
                {
                    recursion = RecursionType.Full;
                }

                var versionControlServer = _command.Connection.VersionControlServer;
                var workspaces = versionControlServer.QueryWorkspaces(null, null, Environment.MachineName);
                Workspace workspace = null;
                WorkingFolder workingFolder = null;

                foreach (var currentWorkspace in workspaces)
                {
                    workingFolder = currentWorkspace.TryGetWorkingFolderForServerItem(path);

                    if (workingFolder != null)
                    {
                        workspace = currentWorkspace;
                        var itemSpec = new ItemSpec(path, recursion);
                        var extendedItems = currentWorkspace.GetExtendedItems(new[] {itemSpec}, DeletedState.Any, ItemType.Any);
                        _items = extendedItems[0];
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
                queryForm.AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null,
                    $"\r\nworkspace.Name: {name}\r\nworkingFolder.LocalItem: {workingFolder.LocalItem}"));
            }

            if (_items != null && _index < _items.Length)
            {
                var item = _items[_index];

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
                _index++;
            }
            else
                read = false;

            return read;
        }

        public override int RecordsAffected => -1;
        public override int FieldCount => 8;
    }
}