using System;
using System.Collections.Generic;
using System.IO;
using Foundation.Assertions;

namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
public sealed class TextDataStreamReader
{
    private readonly TextReader _textReader;

    private readonly IList<TextDataColumn> _columns;

    private readonly IList<ITextDataConverter> _converters;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textReader"></param>
    /// <param name="columns"></param>
    /// <param name="converters"></param>
    public TextDataStreamReader(TextReader textReader, IList<TextDataColumn> columns, IList<ITextDataConverter> converters)
    {
        ArgumentNullException.ThrowIfNull(textReader);
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(converters);

        _textReader = textReader;
        _columns = columns;
        _converters = converters;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public object[] ReadRow()
    {
        object[] values = null;
        var index = 0;

        foreach (var column in _columns)
        {
            var maxLength = column.MaxLength;
            var buffer = new char[maxLength];
            var count = _textReader.Read(buffer, 0, maxLength);

            if (count == 0)
            {
                break;
            }

            Assert.IsTrue(count == maxLength);

            if (index == 0)
                values = new object[_columns.Count];

            var source = new string(buffer);
            var converter = _converters[index];

            Assert.IsTrue(converter != null);

            object value;

            try
            {
                value = converter.FromString(source, column);
            }
            catch (Exception e)
            {
                throw new TextDataFormatException(column, converter, source, e);
            }

            values[index] = value;
            index++;
        }

        return values;
    }
}