using System;
using System.Collections.Generic;
using System.IO;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data.TextData
{
    internal sealed class TextDataStreamWriter
    {
        private readonly TextWriter _textWriter;

        private readonly IList<ITextDataConverter> _converters;

        public TextDataStreamWriter(TextWriter textWriter, IList<TextDataColumn> columns, IList<ITextDataConverter> converters)
        {
            FoundationContract.Requires<ArgumentNullException>(textWriter != null);
            FoundationContract.Requires<ArgumentNullException>(columns != null);
            FoundationContract.Requires<ArgumentNullException>(converters != null);

            _textWriter = textWriter;
            Columns = columns;
            _converters = converters;
        }

        public IList<TextDataColumn> Columns { get; }

        public void WriteRow(object[] values)
        {
            FoundationContract.Requires<ArgumentNullException>(values != null);
            FoundationContract.Requires<ArgumentNullException>(this.Columns.Count == values.Length);

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var converter = _converters[i];
                var column = Columns[i];
                var valueString = converter.ToString(value, column);

                FoundationContract.Assert(!string.IsNullOrEmpty(valueString));
                FoundationContract.Assert(column.MaxLength == valueString.Length);

                _textWriter.Write(valueString);
            }
        }
    }
}