using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
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

        public static List<T> Read<T>(this IDataReader dataReader, Func<IDataReader, T> read)
        {
            var objects = new List<T>();

            dataReader.Read(() =>
            {
                var @object = read(dataReader);
                objects.Add(@object);
            });

            return objects;
        }

        public static List<T> ReadNext<T>(this IDataReader dataReader, Func<IDataReader, T> read)
        {
            var nextResult = dataReader.NextResult();
            FoundationContract.Assert(nextResult);
            return dataReader.Read(read);
        }

        public static List<T> Read<T>(this IDataReader dataReader, Func<T> read)
        {
            var objects = new List<T>();

            dataReader.Read(() =>
            {
                var @object = read();
                objects.Add(@object);
            });

            return objects;
        }

        public static ExecuteReaderResponse<T1, T2> Read<T1, T2>(this IDataReader dataReader, Func<T1> read1, Func<T2> read2)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;

            var reads = new Action[]
            {
                () => objects1 = dataReader.Read(read1),
                () => objects2 = dataReader.Read(read2)
            };

            dataReader.Read(reads);

            return ExecuteReaderResponse.Create(objects1, objects2);
        }

        public static ExecuteReaderResponse<T1, T2, T3> Read<T1, T2, T3>(this IDataReader dataReader, Func<T1> read1, Func<T2> read2, Func<T3> read3)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;
            List<T3> objects3 = null;

            var reads = new Action[]
            {
                () => objects1 = dataReader.Read(read1),
                () => objects2 = dataReader.Read(read2),
                () => objects3 = dataReader.Read(read3)
            };

            dataReader.Read(reads);

            return ExecuteReaderResponse.Create(objects1, objects2, objects3);
        }

        public static ExecuteReaderResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
            Read<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(this IDataReader dataReader, Func<T1> read1, Func<T2> read2, Func<T3> read3,
                Func<T4> read4, Func<T5> read5, Func<T6> read6, Func<T7> read7, Func<T8> read8, Func<T9> read9, Func<T10> read10, Func<T11> read11, Func<T12> read12,
                Func<T13> read13, Func<T14> read14, Func<T15> read15, Func<T16> read16, Func<T17> read17, Func<T18> read18, Func<T19> read19)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;
            List<T3> objects3 = null;
            List<T4> objects4 = null;
            List<T5> objects5 = null;
            List<T6> objects6 = null;
            List<T7> objects7 = null;
            List<T8> objects8 = null;
            List<T9> objects9 = null;
            List<T10> objects10 = null;
            List<T11> objects11 = null;
            List<T12> objects12 = null;
            List<T13> objects13 = null;
            List<T14> objects14 = null;
            List<T15> objects15 = null;
            List<T16> objects16 = null;
            List<T17> objects17 = null;
            List<T18> objects18 = null;
            List<T19> objects19 = null;

            var reads = new Action[]
            {
                () => objects1 = dataReader.Read(read1),
                () => objects2 = dataReader.Read(read2),
                () => objects3 = dataReader.Read(read3),
                () => objects4 = dataReader.Read(read4),
                () => objects5 = dataReader.Read(read5),
                () => objects6 = dataReader.Read(read6),
                () => objects7 = dataReader.Read(read7),
                () => objects8 = dataReader.Read(read8),
                () => objects9 = dataReader.Read(read9),
                () => objects10 = dataReader.Read(read10),
                () => objects11 = dataReader.Read(read11),
                () => objects12 = dataReader.Read(read12),
                () => objects13 = dataReader.Read(read13),
                () => objects14 = dataReader.Read(read14),
                () => objects15 = dataReader.Read(read15),
                () => objects16 = dataReader.Read(read16),
                () => objects17 = dataReader.Read(read17),
                () => objects18 = dataReader.Read(read18),
                () => objects19 = dataReader.Read(read19)
            };

            dataReader.Read(reads);

            return ExecuteReaderResponse.Create(objects1, objects2, objects3, objects4, objects5, objects6, objects7, objects8, objects9, objects10, objects11, objects12,
                objects13, objects14, objects15, objects16, objects17, objects18, objects19);
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