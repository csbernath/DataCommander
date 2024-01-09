using System;
using System.Diagnostics;

namespace Foundation.Data.TextData;

[DebuggerDisplay("ColumnName: {ColumnName}, DataType: {DataType}")]
public class TextDataColumn(
    string columnName,
    string caption,
    int maxLength,
    Type dataType,
    byte? numericPrecision,
    byte? numericScale,
    string format,
    IFormatProvider formatProvider)
{
    #region Private Fields

    #endregion

    public Type DataType { get; } = dataType;
    public string Format { get; } = format;
    public IFormatProvider FormatProvider { get; } = formatProvider;
    public int MaxLength { get; } = maxLength;
    public string ColumnName { get; } = columnName;
    public string Caption { get; } = caption;
    public byte? NumericPrecision { get; } = numericPrecision;
    public byte? NumericScale { get; } = numericScale;
}