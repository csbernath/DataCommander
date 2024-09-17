using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class DbDataReaderAsyncExtensions
{
    public static async Task ReadResultAsync(this DbDataReader dataReader, Action readRecord, CancellationToken cancellationToken)
    {
        while (await dataReader.ReadAsync(cancellationToken))
            readRecord();
    }

    public static async Task<ReadOnlySegmentLinkedList<T>> ReadResultAsync<T>(this DbDataReader dataReader, int segmentLength,
        Func<IDataRecord, T> readRecord, CancellationToken cancellationToken)
    {
        SegmentLinkedListBuilder<T> segmentLinkedListBuilder = new SegmentLinkedListBuilder<T>(segmentLength);
        await dataReader.ReadResultAsync(() =>
        {
            T record = readRecord(dataReader);
            segmentLinkedListBuilder.Add(record);
        }, cancellationToken);
        return segmentLinkedListBuilder.ToReadOnlySegmentLinkedList();
    }

    public static async Task<ReadOnlySegmentLinkedList<T>> ReadNextResultAsync<T>(this DbDataReader dataReader, int segmentLength,
        Func<IDataRecord, T> readRecord,
        CancellationToken cancellationToken)
    {
        bool nextResult = await dataReader.NextResultAsync(cancellationToken);
        Assert.IsTrue(nextResult);
        ReadOnlySegmentLinkedList<T> records = await dataReader.ReadResultAsync(segmentLength, readRecord, cancellationToken);
        return records;
    }
}