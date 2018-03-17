using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Foundation.Diagnostics.Assertions;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public static IEnumerable<IDataReader> AsEnumerable(this IDataReader dataReader)
        {
            Assert.IsNotNull(dataReader);

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
            Assert.IsNotNull(dataReader);
            Assert.IsNotNull(dataSet);

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
            Assert.IsNotNull(dataReader);
            Assert.IsNotNull(dataTable);

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