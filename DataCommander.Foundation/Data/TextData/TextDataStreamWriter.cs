namespace DataCommander.Foundation.Data.TextData
{
    using System.Collections.Generic;
    using System.IO;

    internal sealed class TextDataStreamWriter
    {
        private readonly TextWriter _textWriter;

        private readonly IList<ITextDataConverter> _converters;

        public TextDataStreamWriter(TextWriter textWriter, IList<TextDataColumn> columns, IList<ITextDataConverter> converters)
        {
#if CONTRACTS_FULL
            Contract.Requires(textWriter != null);
            Contract.Requires(columns != null);
            Contract.Requires(converters != null);
#endif

            this._textWriter = textWriter;
            this.Columns = columns;
            this._converters = converters;
        }

        public IList<TextDataColumn> Columns { get; }

        public void WriteRow(object[] values)
        {
#if CONTRACTS_FULL
            Contract.Requires(values != null);
            Contract.Requires(this.Columns.Count == values.Length);
#endif

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var converter = this._converters[i];
                var column = this.Columns[i];
                var valueString = converter.ToString(value, column);
#if CONTRACTS_FULL
                Contract.Assert(!string.IsNullOrEmpty(valueString));
                Contract.Assert(column.MaxLength == valueString.Length);
#endif
                this._textWriter.Write(valueString);
            }
        }
    }
}