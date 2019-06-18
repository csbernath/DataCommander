﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using Foundation.Assertions;

namespace Foundation.Data
{
    public static class DataReaderExtensions
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

        public static List<T> ReadResult<T>(this IDataReader dataReader, Func<IDataRecord, T> readRecord)
        {
            var records = new List<T>();

            dataReader.ReadResult(() =>
            {
                var record = readRecord(dataReader);
                records.Add(record);
            });

            return records;
        }

        public static List<T> ReadNextResult<T>(this IDataReader dataReader, Func<IDataRecord, T> readRecord)
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
                var record = readRecord();
                records.Add(record);
            });

            return records;
        }

        public static int Fill(this IDataReader dataReader, DataSet dataSet, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(dataReader);
            Assert.IsNotNull(dataSet);

            var rowCount = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var table = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture
                };

                var count = dataReader.Fill(table, cancellationToken);
                rowCount += count;
                dataSet.Tables.Add(table);

                if (!dataReader.NextResult())
                    break;
            }

            return rowCount;
        }

        public static int Fill(this IDataReader dataReader, DataTable dataTable, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(dataReader);
            Assert.IsNotNull(dataTable);

            var schemaTable = dataReader.GetSchemaTable();

            if (schemaTable != null)
            {
                var columns = dataTable.Columns;

                if (columns.Count == 0)
                    SchemaFiller.FillSchema(schemaTable, dataTable);
            }

            var fieldCount = dataReader.FieldCount;
            var rows = dataTable.Rows;
            var rowCount = 0;

            while (dataReader.Read())
            {
                var values = new object[fieldCount];
                dataReader.GetValues(values);
                var row = rows.Add(values);
                row.AcceptChanges();
                rowCount++;

                cancellationToken.ThrowIfCancellationRequested();
            }

            return rowCount;
        }
    }
}