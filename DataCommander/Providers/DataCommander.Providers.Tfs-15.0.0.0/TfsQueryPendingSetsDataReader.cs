using System;
using System.Collections.Generic;
using System.Data;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsQueryPendingSetsDataReader : TfsDataReader
    {
        private readonly TfsCommand _command;
        private bool _first = true;
        private PendingSet[] _pendingSets;
        private IEnumerator<Tuple<int, int>> _enumerator;

        public TfsQueryPendingSetsDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            this._command = command;
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

            if (_command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (_first)
                {
                    _first = false;
                    var parameters = _command.Parameters;
                    var path = ValueReader.GetValueOrDefault<string>(parameters["path"].Value);
                    var recursionString = ValueReader.GetValueOrDefault<string>(parameters["recursion"].Value);
                    RecursionType recursion;

                    if (recursionString != null)
                    {
                        recursion = Enum<RecursionType>.Parse(recursionString);
                    }
                    else
                    {
                        recursion = RecursionType.Full;
                    }

                    var workspace = ValueReader.GetValueOrDefault<string>(parameters["workspace"].Value);
                    var user = ValueReader.GetValueOrDefault<string>(parameters["user"].Value);
                    _pendingSets = _command.Connection.VersionControlServer.QueryPendingSets(new[] {path}, recursion, workspace, user);
                    _enumerator = AsEnumerable(_pendingSets).GetEnumerator();
                }

                var moveNext = _enumerator.MoveNext();

                if (moveNext)
                {
                    var pair = _enumerator.Current;
                    var pendingSet = _pendingSets[pair.Item1];
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