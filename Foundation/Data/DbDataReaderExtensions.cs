using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
    public static class DbDataReaderExtensions
    {
        private static async Task ReadAsync(this DbDataReader dataReader, Action read, CancellationToken cancellationToken)
        {
            while (await dataReader.ReadAsync(cancellationToken))
                read();
        }

        public static async Task ReadAsync(this DbDataReader dataReader, IEnumerable<Func<Task>> reads, CancellationToken cancellationToken)
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

        public static async Task<List<T>> ReadAsync<T>(this DbDataReader dataReader, Func<IDataRecord, T> read, CancellationToken cancellationToken)
        {
            var records = new List<T>();

            await dataReader.ReadAsync(() =>
            {
                var record = read(dataReader);
                records.Add(record);
            }, cancellationToken);

            return records;
        }

        public static async Task<ExecuteReaderResponse<T1, T2>> ReadAsync<T1, T2>(
            this DbDataReader dataReader,
            Func<IDataRecord, T1> read1,
            Func<IDataRecord, T2> read2,
            CancellationToken cancellationToken)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;

            var readResults = new Func<Task>[]
            {
                async () => objects1 = await dataReader.ReadAsync(read1, cancellationToken),
                async () => objects2 = await dataReader.ReadAsync(read2, cancellationToken),
            };

            await dataReader.ReadAsync(readResults, cancellationToken);

            return ExecuteReaderResponse.Create(objects1, objects2);
        }

        public static async Task<ExecuteReaderResponse<T1, T2, T3>> ReadAsync<T1, T2, T3>(
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
                async () => objects1 = (await dataReader.ReadAsync(read1, cancellationToken)),
                async () => objects2 = (await dataReader.ReadAsync(read2, cancellationToken)),
                async () => objects3 = (await dataReader.ReadAsync(read3, cancellationToken)),
            };

            await dataReader.ReadAsync(reads, cancellationToken);

            return ExecuteReaderResponse.Create(objects1, objects2, objects3);
        }
    }
}