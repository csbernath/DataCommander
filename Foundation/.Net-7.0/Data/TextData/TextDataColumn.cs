using System;
using System.Diagnostics;

namespace Foundation.Data.TextData;

[DebuggerDisplay("ColumnName: {ColumnName}, DataType: {DataType}")]
public class TextDataColumn
{
    #region Private Fields

    #endregion

    public TextDataColumn(string columnName, string caption, int maxLength, Type dataType, byte? numericPrecision, byte? numericScale, string format,
        IFormatProvider formatProvider)
    {
        ColumnName = columnName;
        Caption = caption;
        MaxLength = maxLength;
        DataType = dataType;
        NumericPrecision = numericPrecision;
        NumericScale = numericScale;
        Format = format;
        FormatProvider = formatProvider;
    }

    public Type DataType { get; }
    public string Format { get; }
    public IFormatProvider FormatProvider { get; }
    public int MaxLength { get; }
    public string ColumnName { get; }
    public string Caption { get; }
    public byte? NumericPrecision { get; }
    public byte? NumericScale { get; }
}