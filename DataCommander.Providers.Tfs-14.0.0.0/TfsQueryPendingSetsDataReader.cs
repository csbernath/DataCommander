using Foundation;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsQueryPendingSetsDataReader : TfsDataReader
    {
        private readonly TfsCommand command;
        private bool first = true;
        private PendingSet[] pendingSets;
        private IEnumerator<Tuple<int, int>> enumerator;

        public TfsQueryPendingSetsDataReader(TfsCommand command)
        {
            FoundationContract.Requires<ArgumentNullException>(command != null);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
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

            if (command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (first)
                {
                    first = false;
                    var parameters = command.Parameters;
                    var path = Database.GetValueOrDefault<string>(parameters["path"].Value);
                    var recursionString = Database.GetValueOrDefault<string>(parameters["recursion"].Value);
                    RecursionType recursion;

                    if (recursionString != null)
                    {
                        recursion = Enum<RecursionType>.Parse(recursionString);
                    }
                    else
                    {
                        recursion = RecursionType.Full;
                    }

                    var workspace = Database.GetValueOrDefault<string>(parameters["workspace"].Value);
                    var user = Database.GetValueOrDefault<string>(parameters["user"].Value);
                    pendingSets = command.Connection.VersionControlServer.QueryPendingSets(new string[] {path}, recursion, workspace, user);
                    enumerator = AsEnumerable(pendingSets).GetEnumerator();
                }

                var moveNext = enumerator.MoveNext();

                if (moveNext)
                {
                    var pair = enumerator.Current;
                    var pendingSet = pendingSets[pair.Item1];
                    var pendingChange = pendingSet.PendingChanges[pair.Item2];

                    var values = new object[]
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

                    Values = values;
                    read = true;
                }
                else
                {
                    read = false;
                }
            }

            return read;
        }

        public override int RecordsAffected => -1;

        public override int FieldCount => 11;

        private static IEnumerable<Tuple<int, int>> AsEnumerable(PendingSet[] pendingSets)
        {
            for (var i = 0; i < pendingSets.Length; i++)
            {
                var pendingSet = pendingSets[i];
                var pendingChanges = pendingSet.PendingChanges;

                for (var j = 0; j < pendingChanges.Length; j++)
                {
                    var pair = Tuple.Create(i, j);
                    yield return pair;
                }
            }
        }
    }
}