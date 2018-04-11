using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Assertions;

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

        public static async Task<List<T>> ReadResultAsync<T>(this DbDataReader dataReader, Func<IDataRecord, T> readRecord, CancellationToken cancellationToken)
        {
            var records = new List<T>();

            await dataReader.ReadResultAsync(() =>
            {
                var record = readRecord(dataReader);
                records.Add(record);
            }, cancellationToken);

            return records;
        }

        public static async Task<List<T>> ReadNextResultAsync<T>(this DbDataReader dataReader, Func<IDataRecord, T> readRecord, CancellationToken cancellationToken)
        {
            var nextResult = await dataReader.NextResultAsync(cancellationToken);
            Assert.IsTrue(nextResult);
            var records = await dataReader.ReadResultAsync(readRecord, cancellationToken);
            return records;
        }

        public static async Task<ExecuteReaderResponse<T1, T2>> ReadResultAsync<T1, T2>(
            this DbDataReader dataReader,
            Func<IDataRecord, T1> read1,
            Func<IDataRecord, T2> read2,
            CancellationToken cancellationToken)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;

            var readResults = new Func<Task>[]
            {
                async () => objects1 = await dataReader.ReadResultAsync(read1, cancellationToken),
                async () => objects2 = await dataReader.ReadResultAsync(read2, cancellationToken),
            };

            await dataReader.ReadResultAsync(readResults, cancellationToken);

            return ExecuteReaderResponse.Create(objects1, objects2);
        }

        public static async Task<ExecuteReaderResponse<T1, T2, T3>> ReadResultAsync<T1, T2, T3>(
            this DbDataReader dataReader,
            Func<IDataRecord, T1> read1,
            Func<IDataRecord, T2> read2,
            Func<IDataRecord, T3> read3,
            CancellationToken cancellationToken)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;
            List<T3> objects3 = null;

            var reads = new Func<Task>[]
            {
                async () => objects1 = (await dataReader.ReadResultAsync(read1, cancellationToken)),
                async () => objects2 = (await dataReader.ReadResultAsync(read2, cancellationToken)),
                async () => objects3 = (await dataReader.ReadResultAsync(read3, cancellationToken)),
            };

            await dataReader.ReadResultAsync(reads, cancellationToken);

            return ExecuteReaderResponse.Create(objects1, objects2, objects3);
        }
    }
}