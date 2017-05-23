namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using Diagnostics.Contracts;

    public static class DbDataReaderExtensions
    {
        public static async Task ReadAsync(this DbDataReader dataReader, Action read,
            CancellationToken cancellationToken)
        {
            while (await dataReader.ReadAsync(cancellationToken))
                read();
        }

        public static async Task ReadAsync(this DbDataReader dataReader, IEnumerable<Action> reads,
            CancellationToken cancellationToken)
        {
            foreach (var read in reads)
            {
                await dataReader.ReadAsync(read, cancellationToken);
                var nextResult = await dataReader.NextResultAsync(cancellationToken);
                FoundationContract.Assert(nextResult);
            }
        }

        public static async Task<List<TRow>> ReadAsync<TRow>(this DbDataReader dataReader, Func<IDataRecord, TRow> read, CancellationToken cancellationToken)
        {
            var rows = new List<TRow>();

            await dataReader.ReadAsync(() =>
            {
                var row = read(dataReader);
                rows.Add(row);
            }, cancellationToken);

            return rows;
        }

        public static async Task<ExecuteReaderResponse<TRow1, TRow2>> ReadAsync<TRow1, TRow2>(
            this DbDataReader dataReader,
            Func<IDataRecord, TRow1> read1,
            Func<IDataRecord, TRow2> read2,
            CancellationToken cancellationToken)
        {
            List<TRow1> rows1 = null;
            List<TRow2> rows2 = null;

            var reads = new Action[]
            {
                async () => rows1 = (await dataReader.ReadAsync(read1, cancellationToken)),
                async () => rows2 = (await dataReader.ReadAsync(read2, cancellationToken)),
            };

            await dataReader.ReadAsync(reads, cancellationToken);

            return ExecuteReaderResponse.Create(rows1, rows2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="read1"></param>
        /// <param name="read2"></param>
        /// <param name="read3"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TRow1"></typeparam>
        /// <typeparam name="TRow2"></typeparam>
        /// <typeparam name="TRow3"></typeparam>
        /// <returns></returns>
        public static async Task<ExecuteReaderResponse<TRow1, TRow2, TRow3>> ReadAsync<TRow1, TRow2, TRow3>(
            this DbDataReader dataReader,
            Func<IDataRecord, TRow1> read1,
            Func<IDataRecord, TRow2> read2,
            Func<IDataRecord, TRow3> read3,
            CancellationToken cancellationToken)
        {
            List<TRow1> rows1 = null;
            List<TRow2> rows2 = null;
            List<TRow3> rows3 = null;

            var reads = new Action[]
            {
                async () => rows1 = (await dataReader.ReadAsync(read1, cancellationToken)),
                async () => rows2 = (await dataReader.ReadAsync(read2, cancellationToken)),
                async () => rows3 = (await dataReader.ReadAsync(read3, cancellationToken)),
            };

            await dataReader.ReadAsync(reads, cancellationToken);

            return ExecuteReaderResponse.Create(rows1, rows2, rows3);
        }
    }
}