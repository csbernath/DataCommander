using System;
using System.Collections.Generic;
using System.IO;
using Foundation.Assertions;

namespace Foundation.Data.TextData;

internal sealed class TextDataStreamWriter
{
    private readonly TextWriter _textWriter;

    private readonly IList<ITextDataConverter> _converters;

    public TextDataStreamWriter(TextWriter textWriter, IList<TextDataColumn> columns, IList<ITextDataConverter> converters)
    {
        ArgumentNullException.ThrowIfNull(textWriter);
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(converters);

        _textWriter = textWriter;
        Columns = columns;
        _converters = converters;
    }

    public IList<TextDataColumn> Columns { get; }

    public void WriteRow(object[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        Assert.IsTrue(Columns.Count == values.Length);

        for (int i = 0; i < values.Length; i++)
        {
            object value = values[i];
            ITextDataConverter converter = _converters[i];
            TextDataColumn column = Columns[i];
            string valueString = converter.ToString(value, column);

            Assert.IsTrue(!string.IsNullOrEmpty(valueString));
            Assert.IsTrue(column.MaxLength == valueString.Length);

            _textWriter.Write(valueString);
        }
    }
}