using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsQueryWorkspacesDataReader : TfsDataReader
    {
        private static DataTable schemaTable;
        private readonly TfsCommand command;
        private bool first = true;
        private Workspace[] workspaces;
        private IEnumerator<Tuple<int, int>> enumerator;
        private int index;

        static TfsQueryWorkspacesDataReader()
        {
            schemaTable = CreateSchemaTable();
            AddSchemaRowString(schemaTable, "Computer", false);
            AddSchemaRowString(schemaTable, "OwnerName", false);
            AddSchemaRowString(schemaTable, "Name", false);
            AddSchemaRowString(schemaTable, "Comment", false);
            AddSchemaRowString(schemaTable, "DisplayName", false);
            AddSchemaRowString(schemaTable, "FolderType", false);
            AddSchemaRowBoolean(schemaTable, "IsCloaked", false);
            AddSchemaRowString(schemaTable, "FolderServerItem", false);
            AddSchemaRowString(schemaTable, "FolderLocalItem", false);
        }

        public TfsQueryWorkspacesDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            return schemaTable;
        }

        public override bool Read()
        {
            bool read;

            if (first)
            {
                first = false;
                var parameters = command.Parameters;
                var workspace = Database.GetValueOrDefault<string>(parameters["workspace"].Value);
                var owner = Database.GetValueOrDefault<string>(parameters["owner"].Value);
                var computer = Database.GetValueOrDefault<string>(parameters["computer"].Value);
                workspaces = command.Connection.VersionControlServer.QueryWorkspaces(workspace, owner, computer);
                enumerator = AsEnumerable(workspaces).GetEnumerator();
            }

            var moveNext = enumerator.MoveNext();

            if (moveNext)
            {
                var pair = enumerator.Current;
                var workspace = workspaces[pair.Item1];
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
                index++;
            }
            else
            {
                read = false;
            }

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
                {
                    for (var j = 0; j < folders.Length; j++)
                    {
                        yield return Tuple.Create(i, j);
                    }
                }
                else
                {
                    yield return Tuple.Create(i, -1);
                }
            }
        }
    }
}