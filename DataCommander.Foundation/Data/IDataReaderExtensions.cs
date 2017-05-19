namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class IDataReaderExtensions
    {
        public static void Read(this IDataReader dataReader, Action read)
        {
            while (dataReader.Read())
                read();
        }

        public static void Read(this IDataReader dataReader, IEnumerable<Action> reads)
        {
            foreach (var read in reads)
            {
                dataReader.Read(read);
                var nextResult = dataReader.NextResult();
                FoundationContract.Assert(nextResult);
            }
        }

        public static ExecuteReaderResponse<TRow> Read<TRow>(this IDataReader dataReader, Func<IDataRecord, TRow> read)
        {
            var rows = new List<TRow>();

            dataReader.Read(() =>
            {
                var row = read(dataReader);
                rows.Add(row);
            });

            return ExecuteReaderResponse.Create(rows);
        }

        public static ExecuteReaderResponse<TRow1, TRow2> Read<TRow1, TRow2>(this IDataReader dataReader, Func<IDataRecord, TRow1> read1,
            Func<IDataRecord, TRow2> read2)
        {
            List<TRow1> rows1 = null;
            List<TRow2> rows2 = null;

            var reads = new Action[]
            {
                () => rows1 = dataReader.Read(read1).Rows,
                () => rows2 = dataReader.Read(read2).Rows,
            };

            dataReader.Read(reads);

            return ExecuteReaderResponse.Create(rows1, rows2);
        }

        public static ExecuteReaderResponse<TRow1, TRow2, TRow3> Read<TRow1, TRow2, TRow3>(this IDataReader dataReader, Func<IDataRecord, TRow1> read1,
            Func<IDataRecord, TRow2> read2, Func<IDataRecord, TRow3> read3)
        {
            List<TRow1> rows1 = null;
            List<TRow2> rows2 = null;
            List<TRow3> rows3 = null;

            var reads = new Action[]
            {
                () => rows1 = dataReader.Read(read1).Rows,
                () => rows2 = dataReader.Read(read2).Rows,
                () => rows3 = dataReader.Read(read3).Rows
            };

            dataReader.Read(reads);

            return ExecuteReaderResponse.Create(rows1, rows2, rows3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static IEnumerable<IDataReader> AsEnumerable(this IDataReader dataReader)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
#endif

            while (dataReader.Read())
            {
                yield return dataReader;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dataSet"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int Fill(this IDataReader dataReader, DataSet dataSet, CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(dataSet != null);
#endif

            var rowCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var table =
                    new DataTable
                    {
                        Locale = CultureInfo.InvariantCulture
                    };

                var count = dataReader.Fill(table, cancellationToken);
                rowCount += count;
                dataSet.Tables.Add(table);

                if (!dataReader.NextResult())
                {
                    break;
                }
            }

            return rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="dataTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int Fill(this IDataReader dataReader, DataTable dataTable, CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(dataReader != null);
            Contract.Requires<ArgumentNullException>(dataTable != null);
#endif

            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var columns = dataTable.Columns;

                if (columns.Count == 0)
                {
                    Database.FillSchema(schemaTable, dataTable);
                }
            }

            var fieldCount = dataReader.FieldCount;
            var rows = dataTable.Rows;
            var rowCount = 0;

            while (!cancellationToken.IsCancellationRequested && dataReader.Read())
            {
                var values = new object[fieldCount];
                dataReader.GetValues(values);
                var row = rows.Add(values);
                row.AcceptChanges();
                rowCount++;
            }

            return rowCount;
        }
    }
}