using System;
using System.Data;
using System.Globalization;
using System.Threading;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class IDataReaderExtensions
{
    public static ReadOnlySegmentLinkedList<T> ReadResult<T>(this IDataReader dataReader, int segmentLength, Func<IDataRecord, T> readRecord)
    {
        SegmentLinkedListBuilder<T> segmentLinkedListBuilder = new SegmentLinkedListBuilder<T>(segmentLength);
        while (dataReader.Read())
        {
            T record = readRecord(dataReader);
            segmentLinkedListBuilder.Add(record);
        }

        return segmentLinkedListBuilder.ToReadOnlySegmentLinkedList();
    }

    public static ReadOnlySegmentLinkedList<T> ReadNextResult<T>(this IDataReader dataReader, int segmentLength, Func<IDataRecord, T> readRecord)
    {
        bool nextResult = dataReader.NextResult();
        Assert.IsTrue(nextResult);
        return dataReader.ReadResult(segmentLength, readRecord);
    }

    public static T ReadScalar<T>(this IDataReader dataReader, Func<IDataRecord, T> readScalar)
    {
        bool read = dataReader.Read();
        Assert.IsTrue(read);

        T scalar = readScalar(dataReader);

        read = dataReader.Read();
        Assert.IsTrue(!read);

        return scalar;
    }

    public static T ReadNextScalar<T>(this IDataReader dataReader, Func<IDataRecord, T> readScalar)
    {
        bool nextResult = dataReader.NextResult();
        Assert.IsTrue(nextResult);

        T scalar = dataReader.ReadScalar(readScalar);
        return scalar;
    }

    public static int Fill(this IDataReader dataReader, DataSet dataSet, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dataReader);
        ArgumentNullException.ThrowIfNull(dataSet);

        int rowCount = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataTable table = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };

            int count = dataReader.Fill(table, cancellationToken);
            rowCount += count;
            dataSet.Tables.Add(table);

            if (!dataReader.NextResult())
                break;
        }

        return rowCount;
    }

    public static int Fill(this IDataReader dataReader, DataTable dataTable, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dataReader);
        ArgumentNullException.ThrowIfNull(dataTable);

        DataTable schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            DataColumnCollection columns = dataTable.Columns;

            if (columns.Count == 0)
                SchemaFiller.FillSchema(schemaTable, dataTable);
        }

        int fieldCount = dataReader.FieldCount;
        DataRowCollection rows = dataTable.Rows;
        int rowCount = 0;

        while (dataReader.Read())
        {
            object[] values = new object[fieldCount];
            dataReader.GetValues(values);
            DataRow row = rows.Add(values);
            row.AcceptChanges();
            rowCount++;

            cancellationToken.ThrowIfCancellationRequested();
        }

        return rowCount;
    }
}