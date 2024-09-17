﻿using System;

namespace Foundation.Data.TextData;

public sealed class TextDataRow
{
    private readonly Convert _convert;

    public TextDataRow(TextDataColumnCollection columns, Convert convert)
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(convert);

        Columns = columns;
        _convert = convert;
        ItemArray = new object[columns.Count];

        for (int i = 0; i < ItemArray.Length; i++)
        {
            ItemArray[i] = DBNull.Value;
        }
    }

    public delegate object Convert(object value, TextDataColumn column);

    public object this[string columnName]
    {
        get
        {
            int index = Columns.IndexOf(columnName, true);
            return ItemArray[index];
        }

        set
        {
            int index = Columns.IndexOf(columnName, true);
            TextDataColumn column = Columns[index];
            object convertedValue = _convert(value, column);
            ItemArray[index] = convertedValue;
        }
    }

    public object[] ItemArray { get; }

    public TextDataColumnCollection Columns { get; }

    public object this[TextDataColumn column]
    {
        get
        {
            int index = Columns.IndexOf(column, true);
            return ItemArray[index];
        }

        set
        {
            int index = Columns.IndexOf(column, true);
            object convertedValue = _convert(value, column);
            ItemArray[index] = convertedValue;
        }
    }
}