namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsQueryWorkspacesDataReader : TfsDataReader
    {
        private TfsCommand command;
        private bool first = true;
        private Workspace[] workspaces;
        private IEnumerator<Tuple<int, int>> enumerator;
        private int index;
        private int recordsAffected;

        public TfsQueryWorkspacesDataReader( TfsCommand command )
        {
            Contract.Requires(command != null);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            DataTable table = CreateSchemaTable();
            AddSchemaRowString( table, "Computer", false );
            AddSchemaRowString( table, "OwnerName", false );
            AddSchemaRowString( table, "Name", false );
            AddSchemaRowString( table, "Comment", false );
            AddSchemaRowString( table, "DisplayName", false );
            AddSchemaRowString( table, "FolderType", false );
            AddSchemaRowBoolean( table, "IsCloaked", false );
            AddSchemaRowString( table, "FolderServerItem", false );
            AddSchemaRowString( table, "FolderLocalItem", false );
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (this.first)
            {
                this.first = false;
                TfsParameterCollection parameters = this.command.Parameters;
                string workspace = Database.GetValueOrDefault<string>( parameters[ "workspace" ].Value );
                string owner = Database.GetValueOrDefault<string>( parameters[ "owner" ].Value );
                string computer = Database.GetValueOrDefault<string>( parameters[ "computer" ].Value );
                this.workspaces = this.command.Connection.VersionControlServer.QueryWorkspaces( workspace, owner, computer );
                this.enumerator = AsEnumerable( this.workspaces ).GetEnumerator();
            }

            bool moveNext = this.enumerator.MoveNext();

            if (moveNext)
            {
                Tuple<int, int> pair = this.enumerator.Current;
                Workspace workspace = this.workspaces[ pair.Item1 ];
                int folderIndex = pair.Item2;

                object[] values = new object[]
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
                    WorkingFolder folder = workspace.Folders[ folderIndex ];
                    values[ 5 ] = folder.Type.ToString();
                    values[ 6 ] = folder.IsCloaked;
                    values[ 7 ] = folder.ServerItem;
                    values[ 8 ] = folder.LocalItem;
                }

                this.Values = values;
                read = true;
                this.recordsAffected++;
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
                return this.recordsAffected;
            }
        }

        public override int FieldCount
        {
            get
            {
                return 9;
            }
        }

        private static IEnumerable<Tuple<int, int>> AsEnumerable( Workspace[] workspaces )
        {
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();

            for (int i = 0; i < workspaces.Length; i++)
            {
                Workspace workspace = workspaces[ i ];
                WorkingFolder[] folders = workspace.Folders;

                if (folders.Length > 0)
                {
                    for (int j = 0; j < folders.Length; j++)
                    {
                        yield return new Tuple<int, int>( i, j );
                    }
                }
                else
                {
                    yield return new Tuple<int, int>( i, -1 );
                }
            }
        }
    }
}