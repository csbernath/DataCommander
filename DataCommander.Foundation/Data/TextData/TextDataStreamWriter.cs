namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    internal sealed class TextDataStreamWriter
    {
        private TextWriter textWriter;

        private readonly IList<TextDataColumn> columns;

        private IList<ITextDataConverter> converters;

        public TextDataStreamWriter( TextWriter textWriter, IList<TextDataColumn> columns, IList<ITextDataConverter> converters )
        {
            Contract.Requires( textWriter != null );
            Contract.Requires( columns != null );
            Contract.Requires( converters != null );

            this.textWriter = textWriter;
            this.columns = columns;
            this.converters = converters;
        }

        public IList<TextDataColumn> Columns
        {
            get
            {
                return this.columns;
            }
        }

        public void WriteRow( Object[] values )
        {
            Contract.Requires( values != null );
            Contract.Requires( this.Columns.Count == values.Length );

            for (Int32 i = 0; i < values.Length; i++)
            {
                Object value = values[ i ];
                ITextDataConverter converter = this.converters[ i ];
                TextDataColumn column = this.columns[ i ];
                String valueString = converter.ToString( value, column );
                Contract.Assert( !string.IsNullOrEmpty( valueString ) );
                Contract.Assert( column.MaxLength == valueString.Length );
                this.textWriter.Write( valueString );
            }
        }
    }
}