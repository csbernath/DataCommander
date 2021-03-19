using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    internal class TfsQueryHistoryDataReader : TfsDataReader
    {
        #region Private Fields

        private readonly TfsCommand _command;
        private bool _first = true;
        private IEnumerator<Tuple<Changeset, int>> _enumerator;
        private int _recordCount;

        #endregion

        public TfsQueryHistoryDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);

            _command = command;
        }

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
            AddSchemaRowInt32(table, "ChangesetId", false);
            AddSchemaRowString(table, "Committer", false);
            AddSchemaRowDateTime(table, "CreationDate", false);
            AddSchemaRowString(table, "Comment", false);
            AddSchemaRowString(table, "ChangeType", true);
            AddSchemaRowString(table, "ServerItem", true);
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
                var version = VersionSpec.Latest;
                const int deletionId = 0;
                var parameter = parameters.FirstOrDefault(p => p.ParameterName == "recursion");
                RecursionType recursion;
                if (parameter != null && parameter.Value != null && parameter.Value != DBNull.Value)
                    recursion = Enum<RecursionType>.Parse((string) parameter.Value);
                else
                    recursion = RecursionType.Full;

                parameter = parameters.FirstOrDefault(p => p.ParameterName == "user");
                var user = parameter != null ? ValueReader.GetValueOrDefault<string>(parameter.Value) : null;
                VersionSpec versionFrom = null;
                VersionSpec versionTo = null;
                parameter = parameters.FirstOrDefault(p => p.ParameterName == "maxCount");
                int maxCount;
                if (parameter != null)
                {
                    maxCount = ValueReader.GetValueOrDefault<int>(parameter.Value);
                    if (maxCount == 0)
                        maxCount = (int) parameter.DefaultValue;
                }
                else
                    maxCount = (int) TfsDataReaderFactory.Dictionary[_command.CommandText].Parameters["maxCount"].DefaultValue;

                parameter = parameters.FirstOrDefault(p => p.ParameterName == "includeChanges");
                var includeChanges = parameter != null ? ValueReader.GetValueOrDefault<bool>(parameters["includeChanges"].Value) : false;
                var slotMode = ValueReader.GetValueOrDefault<bool>(parameters["slotMode"].Value);
                var changesets = _command.Connection.VersionControlServer.QueryHistory(path, version, deletionId, recursion, user, versionFrom, versionTo, maxCount, includeChanges,
                    slotMode);
                _enumerator = AsEnumerable(changesets).GetEnumerator();
            }

            var moveNext = _enumerator.MoveNext();

            if (moveNext)
            {
                var pair = _enumerator.Current;
                var changeset = pair.Item1;

                var values = new object[]
                {
                    changeset.ChangesetId,
                    changeset.Committer,
                    changeset.CreationDate,
                    changeset.Comment,
                    null,
                    null
                };

                var changeIndex = pair.Item2;

                if (changeIndex >= 0)
                {
                    var change = changeset.Changes[changeIndex];
                    values[4] = change.ChangeType;
                    values[5] = change.Item.ServerItem;
                }

                Values = values;
                _recordCount++;
                read = true;
            }
            else
            {
                read = false;
            }

            return read;
        }

        public override int RecordsAffected => -1;
        public override int FieldCount => 6;

        private IEnumerable<Tuple<Changeset, int>> AsEnumerable(IEnumerable changesets)
        {
            foreach (Changeset changeset in changesets)
            {
                var changes = changeset.Changes;

                if (changes.Length > 0)
                {
                    for (var i = 0; i < changes.Length; i++)
                    {
                        yield return new Tuple<Changeset, int>(changeset, i);
                    }
                }
                else
                {
                    yield return new Tuple<Changeset, int>(changeset, -1);
                }
            }
        }
    }
}