using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using Foundation.Assertions;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace DataCommander.Providers.Tfs
{
    internal class TfsDownloadItemVersionsDataReader : TfsDataReader
    {
        private readonly TfsCommand _command;
        private string _path;
        private string _localPath;
        private bool _first = true;
        private IEnumerator<Tuple<Changeset, int>> _enumerator;

        public TfsDownloadItemVersionsDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            _command = command;
        }

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
            AddSchemaRowInt32( table, "ChangesetId", false );
            AddSchemaRowString( table, "Committer", false );
            AddSchemaRowDateTime( table, "CreationDate", false );
            AddSchemaRowString( table, "Comment", false );
            AddSchemaRowString( table, "ChangeType", false );
            AddSchemaRowString( table, "ServerItem", false );
            return table;
        }

        //private static int Count( IEnumerable enumerable )
        //{
        //    Assert.IsNotNull( enumerable, "enumerable" );
        //    IEnumerator enumerator = enumerable.GetEnumerator();
        //    int count = 0;

        //    while (enumerator.MoveNext())
        //    {
        //        count++;
        //    }

        //    return count;
        //}

        public override bool Read()
        {
            bool read;

            if (_first)
            {
                _first = false;
                var parameters = _command.Parameters;
                _path = (string) parameters[ "path" ].Value;
                var version = VersionSpec.Latest;
                var deletionId = 0;
                var recursion = RecursionType.None;
                var user = ValueReader.GetValueOrDefault<string>( parameters[ "user" ].Value );
                VersionSpec versionFrom = null;
                VersionSpec versionTo = null;
                var parameter = parameters[ "maxCount" ];

                var maxCount = ValueReader.GetValueOrDefault<int>( parameter.Value );

                if (maxCount == 0)
                {
                    maxCount = (int) parameter.DefaultValue;
                }

                const bool includeChanges = true;
                var slotMode = ValueReader.GetValueOrDefault<bool>( parameters[ "slotMode" ].Value );
                _localPath = ValueReader.GetValueOrDefault<string>( parameters[ "localPath" ].Value );

                if (string.IsNullOrEmpty(_localPath ))
                {
                    _localPath = Path.GetTempPath();
                    _localPath += Path.DirectorySeparatorChar;
                    _localPath += $"getversions [{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff")}]";
                    Directory.CreateDirectory(_localPath );
                }

                var changesets = _command.Connection.VersionControlServer.QueryHistory(_path, version, deletionId, recursion, user, versionFrom, versionTo, maxCount, includeChanges, slotMode );
                _enumerator = AsEnumerable( changesets ).GetEnumerator();
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
                    var change = changeset.Changes[ changeIndex ];
                    values[ 4 ] = change.ChangeType;
                    values[ 5 ] = change.Item.ServerItem;
                }

                Values = values;
                var changesetId = changeset.ChangesetId;
                var changeType = string.Empty;

                for (var i = 0; i < changeset.Changes.Length; i++)
                {
                    var change = changeset.Changes[ i ];
                    Trace.WriteLine( change.ChangeType );
                    Trace.WriteLine( change.Item.ServerItem );
                    _path = change.Item.ServerItem;

                    if (i > 0)
                    {
                        changeType += ',';
                    }

                    changeType += change.ChangeType;
                }

                var creationDate = changeset.CreationDate;
                var deletionId = 0;
                var versionSpec = new ChangesetVersionSpec( changesetId );
                var fileName = VersionControlPath.GetFileName( _path );
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension( fileName );
                var extension = VersionControlPath.GetExtension( _path );
                var localFileName = _localPath + Path.DirectorySeparatorChar + changesetId.ToString().PadLeft( 5, '0' ) + ';' + changeType + ';' + changeset.Committer.Replace( '\\', ' ' ) + extension;
                _command.Connection.VersionControlServer.DownloadFile( _path, deletionId, versionSpec, localFileName );
                File.SetLastWriteTime( localFileName, creationDate );
                File.SetAttributes( localFileName, FileAttributes.ReadOnly );

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

        private IEnumerable<Tuple<Changeset, int>> AsEnumerable( IEnumerable changesets )
        {
            foreach (Changeset changeset in changesets)
            {
                var changes = changeset.Changes;

                if (changes.Length > 0)
                {
                    for (var i = 0; i < changes.Length; i++)
                    {
                        yield return Tuple.Create( changeset, i );
                    }
                }
                else
                {
                    yield return Tuple.Create( changeset, -1 );
                }
            }
        }
    }
}