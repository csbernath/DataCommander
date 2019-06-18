using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data
{
    public static class DbDataReaderExtensions
    {
        public static async Task ReadResultAsync(this DbDataReader dataReader, Action readRecord, CancellationToken cancellationToken)
        {
            while (await dataReader.ReadAsync(cancellationToken))
                readRecord();
        }

        public static async Task ReadResultAsync(this DbDataReader dataReader, IEnumerable<Func<Task>> reads, CancellationToken cancellationToken)
        {
            var first = true;
            foreach (var read in reads)
            {
                if (first)
                    first = false;
                else
                {
                    var nextResult = await dataReader.NextResultAsync(cancellationToken);
                    Assert.IsTrue(nextResult);
                }

                await read();
            }
        }

        public static async Task<ReadOnlySegmentLinkedList<T>> ReadResultAsync<T>(this DbDataReader dataReader, int segmentLength,
            Func<IDataRecord, T> readRecord, CancellationToken cancellationToken)
        {
            var segmentLinkedListBuilder = new SegmentLinkedListBuilder<T>(segmentLength);
            await dataReader.ReadResultAsync(() =>
            {
                var record = readRecord(dataReader);
                segmentLinkedListBuilder.Add(record);
            }, cancellationToken);
            return segmentLinkedListBuilder.ToReadOnlySegmentLinkedList();
        }

        public static async Task<ReadOnlySegmentLinkedList<T>> ReadNextResultAsync<T>(this DbDataReader dataReader, int segmentLength,
            Func<IDataRecord, T> readRecord,
            CancellationToken cancellationToken)
        {
            var nextResult = await dataReader.NextResultAsync(cancellationToken);
            Assert.IsTrue(nextResult);
            var records = await dataReader.ReadResultAsync(segmentLength, readRecord, cancellationToken);
            return records;
        }
    }
}