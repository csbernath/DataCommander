using Foundation;
using Foundation.Data;
using Foundation.Diagnostics.Assertions;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal class TfsQueryHistoryDataReader : TfsDataReader
    {
        #region Private Fields

        private readonly TfsCommand command;
        private bool first = true;
        private IEnumerator<Tuple<Changeset, int>> enumerator;
        private int recordCount;

        #endregion

        public TfsQueryHistoryDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);

            this.command = command;
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

            if (first)
            {
                first = false;
                var parameters = command.Parameters;
                var path = (string) parameters["path"].Value;
                var version = VersionSpec.Latest;
                var deletionId = 0;
                var parameter = parameters.FirstOrDefault(p => p.ParameterName == "recursion");
                RecursionType recursion;
                if (parameter != null && parameter.Value != null && parameter.Value != DBNull.Value)
                {
                    recursion = Enum<RecursionType>.Parse((string) parameter.Value);
                }
                else
                {
                    recursion = RecursionType.Full;
                }

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
                    {
                        maxCount = (int) parameter.DefaultValue;
                    }
                }
                else
                {
                    maxCount = (int) TfsDataReaderFactory.Dictionary[command.CommandText].Parameters["maxCount"].DefaultValue;
                }

                parameter = parameters.FirstOrDefault(p => p.ParameterName == "includeChanges");
                var includeChanges = parameter != null ? ValueReader.GetValueOrDefault<bool>(parameters["includeChanges"].Value) : false;
                var slotMode = ValueReader.GetValueOrDefault<bool>(parameters["slotMode"].Value);
                var changesets = command.Connection.VersionControlServer.QueryHistory(path, version, deletionId, recursion, user, versionFrom, versionTo, maxCount, includeChanges,
                    slotMode);
                enumerator = AsEnumerable(changesets).GetEnumerator();
            }

            var moveNext = enumerator.MoveNext();

            if (moveNext)
            {
                var pair = enumerator.Current;
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
                recordCount++;
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