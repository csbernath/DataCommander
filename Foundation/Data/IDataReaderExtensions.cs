using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDataReaderExtensions
    {
        public static void ReadResult(this IDataReader dataReader, Action readRecord)
        {
            while (dataReader.Read())
                readRecord();
        }

        public static void ReadResults(this IDataReader dataReader, IEnumerable<Action> readRecords)
        {
            foreach (var readRecord in readRecords)
            {
                dataReader.ReadResult(readRecord);
                var nextResult = dataReader.NextResult();
                Assert.IsTrue(nextResult);
            }
        }

        public static List<T> ReadResult<T>(this IDataReader dataReader, Func<IDataReader, T> readRecord)
        {
            var records = new List<T>();

            dataReader.ReadResult(() =>
            {
                var @object = readRecord(dataReader);
                records.Add(@object);
            });

            return records;
        }

        public static List<T> ReadNextResult<T>(this IDataReader dataReader, Func<IDataReader, T> readRecord)
        {
            var nextResult = dataReader.NextResult();
            Assert.IsTrue(nextResult);
            return dataReader.ReadResult(readRecord);
        }

        public static List<T> ReadResult<T>(this IDataReader dataReader, Func<T> readRecord)
        {
            var records = new List<T>();

            dataReader.ReadResult(() =>
            {
                var @object = readRecord();
                records.Add(@object);
            });

            return records;
        }

        public static ExecuteReaderResponse<T1, T2> Read<T1, T2>(this IDataReader dataReader, Func<T1> read1, Func<T2> read2)
        {
            List<T1> records1 = null;
            List<T2> records2 = null;

            var readRecords = new Action[]
            {
                () => records1 = dataReader.ReadResult(read1),
                () => records2 = dataReader.ReadResult(read2)
            };

            dataReader.ReadResults(readRecords);

            return ExecuteReaderResponse.Create(records1, records2);
        }

        public static ExecuteReaderResponse<T1, T2, T3> Read<T1, T2, T3>(this IDataReader dataReader, Func<T1> read1, Func<T2> read2, Func<T3> read3)
        {
            List<T1> objects1 = null;
            List<T2> objects2 = null;
            List<T3> objects3 = null;

            var reads = new Action[]
            {
                () => objects1 = dataReader.ReadResult(read1),
                () => objects2 = dataReader.ReadResult(read2),
                () => objects3 = dataReader.ReadResult(read3)
            };

            dataReader.ReadResults(reads);

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