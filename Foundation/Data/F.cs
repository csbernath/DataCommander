using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class F
{
    public static async Task<ReadOnlySegmentLinkedList<TRecord>> ReadResultAsync<TDataReader, TRecord>(
        this TDataReader dataReader,
        int segmentLength,
        Func<TDataReader, TRecord> readRecord,
        CancellationToken cancellationToken) where TDataReader : DbDataReader
    {
        var segmentLinkedListBuilder = new SegmentLinkedListBuilder<TRecord>(segmentLength);
        await dataReader.ReadResultAsync(() =>
        {
            var record = readRecord(dataReader);
            segmentLinkedListBuilder.Add(record);
        }, cancellationToken);
        return segmentLinkedListBuilder.ToReadOnlySegmentLinkedList();
    }
}