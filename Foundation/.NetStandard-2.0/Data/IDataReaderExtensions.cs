using System;
using System.Data;
using System.Globalization;
using System.Threading;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data
{
    public static class IDataReaderExtensions
    {
        public static ReadOnlySegmentLinkedList<T> ReadResult<T>(this IDataReader dataReader, int segmentLength, Func<IDataRecord, T> readRecord)
        {
            var segmentLinkedListBuilder = new SegmentLinkedListBuilder<T>(segmentLength);
            while (dataReader.Read())
            {
                var record = readRecord(dataReader);
                segmentLinkedListBuilder.Add(record);
            }

            return segmentLinkedListBuilder.ToReadOnlySegmentLinkedList();
        }

        public static ReadOnlySegmentLinkedList<T> ReadNextResult<T>(this IDataReader dataReader, int segmentLength, Func<IDataRecord, T> readRecord)
        {
            var nextResult = dataReader.NextResult();
            Assert.IsTrue(nextResult);
            return dataReader.ReadResult(segmentLength, readRecord);
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