namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Data;
    using Foundation;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsQueryPendingSetsDataReader : TfsDataReader
    {
        private readonly TfsCommand command;
        private bool first = true;
        private PendingSet[] pendingSets;
        private IEnumerator<Tuple<int, int>> enumerator;

        public TfsQueryPendingSetsDataReader(TfsCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            DataTable table = CreateSchemaTable();
            AddSchemaRowString(table, "Computer", false);
            AddSchemaRowString(table, "Name", false);
            AddSchemaRowString(table, "OwnerName", false);
            AddSchemaRowString(table, "PendingSetType", false);
            AddSchemaRowString(table, "ChangeType", false);
            AddSchemaRowDateTime(table, "CreationDate", false);
            AddSchemaRowString(table, "FileName", false);
            AddSchemaRowString(table, "ItemType", false);
            AddSchemaRowString(table, "LockLevelName", false);
            AddSchemaRowString(table, "ServerItem", false);
            AddSchemaRowString(table, "LocalItem", false);
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (this.command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (this.first)
                {
                    this.first = false;
                    var parameters = this.command.Parameters;
                    string path = Database.GetValueOrDefault<string>(parameters["path"].Value);
                    string recursionString = Database.GetValueOrDefault<string>(parameters["recursion"].Value);
                    RecursionType recursion;

                    if (recursionString != null)
                    {
                        recursion = Enum<RecursionType>.Parse(recursionString);
                    }
                    else
                    {
                        recursion = RecursionType.Full;
                    }

                    string workspace = Database.GetValueOrDefault<string>(parameters["workspace"].Value);
                    string user = Database.GetValueOrDefault<string>(parameters["user"].Value);
                    this.pendingSets = this.command.Connection.VersionControlServer.QueryPendingSets(new string[] {path}, recursion, workspace, user);
                    this.enumerator = AsEnumerable(this.pendingSets).GetEnumerator();
                }

                bool moveNext = this.enumerator.MoveNext();

                if (moveNext)
                {
                    Tuple<int, int> pair = this.enumerator.Current;
                    PendingSet pendingSet = this.pendingSets[pair.Item1];
                    PendingChange pendingChange = pendingSet.PendingChanges[pair.Item2];

                    object[] values = new object[]
                    {
                        pendingSet.Computer,
                        pendingSet.Name,
                        pendingSet.OwnerName,
                        pendingSet.Type.ToString(),
                        pendingChange.ChangeType.ToString(),
                        pendingChange.CreationDate,
                        pendingChange.FileName,
                        pendingChange.ItemType.ToString(),
                        pendingChange.LockLevelName,
                        pendingChange.ServerItem,
                        pendingChange.LocalItem
                    };

                    this.Values = values;
                    read = true;
                }
                else
                {
                    read = false;
                }
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
                return 11;
            }
        }

        private static IEnumerable<Tuple<int, int>> AsEnumerable(PendingSet[] pendingSets)
        {
            for (int i = 0; i < pendingSets.Length; i++)
            {
                PendingSet pendingSet = pendingSets[i];
                PendingChange[] pendingChanges = pendingSet.PendingChanges;

                for (int j = 0; j < pendingChanges.Length; j++)
                {
                    var pair = Tuple.Create(i, j);
                    yield return pair;
                }
            }
        }
    }
}