using System;
using System.Collections.Generic;
using System.Data;
using Foundation.Assertions;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsQueryWorkspacesDataReader : TfsDataReader
    {
        private static readonly DataTable SchemaTable;
        private readonly TfsCommand _command;
        private bool _first = true;
        private Workspace[] _workspaces;
        private IEnumerator<Tuple<int, int>> _enumerator;

        static TfsQueryWorkspacesDataReader()
        {
            SchemaTable = CreateSchemaTable();
            AddSchemaRowString(SchemaTable, "Computer", false);
            AddSchemaRowString(SchemaTable, "OwnerName", false);
            AddSchemaRowString(SchemaTable, "Name", false);
            AddSchemaRowString(SchemaTable, "Comment", false);
            AddSchemaRowString(SchemaTable, "DisplayName", false);
            AddSchemaRowString(SchemaTable, "FolderType", false);
            AddSchemaRowBoolean(SchemaTable, "IsCloaked", false);
            AddSchemaRowString(SchemaTable, "FolderServerItem", false);
            AddSchemaRowString(SchemaTable, "FolderLocalItem", false);
        }

        public TfsQueryWorkspacesDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            _command = command;
        }

        public override DataTable GetSchemaTable() => SchemaTable;

        public override bool Read()
        {
            bool read;

            if (_first)
            {
                _first = false;
                var parameters = _command.Parameters;
                var workspace = ValueReader.GetValueOrDefault<string>(parameters["workspace"].Value);
                var owner = ValueReader.GetValueOrDefault<string>(parameters["owner"].Value);
                var computer = ValueReader.GetValueOrDefault<string>(parameters["computer"].Value);
                _workspaces = _command.Connection.VersionControlServer.QueryWorkspaces(workspace, owner, computer);
                _enumerator = AsEnumerable(_workspaces).GetEnumerator();
            }

            var moveNext = _enumerator.MoveNext();

            if (moveNext)
            {
                var pair = _enumerator.Current;
                var workspace = _workspaces[pair.Item1];
                var folderIndex = pair.Item2;

                var values = new object[]
                {
                    workspace.Computer,
                    workspace.OwnerName,
                    workspace.Name,
                    workspace.Comment,
                    workspace.DisplayName,
                    null,
                    null,
                    null,
                    null
                };

                if (folderIndex >= 0)
                {
                    var folder = workspace.Folders[folderIndex];
                    values[5] = folder.Type.ToString();
                    values[6] = folder.IsCloaked;
                    values[7] = folder.ServerItem;
                    values[8] = folder.LocalItem;
                }

                Values = values;
                read = true;
            }
            else
                read = false;

            return read;
        }

        public override int RecordsAffected => -1;
        public override int FieldCount => 9;

        private static IEnumerable<Tuple<int, int>> AsEnumerable(Workspace[] workspaces)
        {
            for (var i = 0; i < workspaces.Length; i++)
            {
                var workspace = workspaces[i];
                var folders = workspace.Folders;

                if (folders.Length > 0)
                    for (var j = 0; j < folders.Length; j++)
                        yield return Tuple.Create(i, j);
                else
                    yield return Tuple.Create(i, -1);
            }
        }
    }
}