namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    internal sealed class TextDataStreamWriter
    {
        private readonly TextWriter textWriter;

        private readonly IList<TextDataColumn> columns;

        private readonly IList<ITextDataConverter> converters;

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

        public void WriteRow( object[] values )
        {
            Contract.Requires( values != null );
            Contract.Requires( this.Columns.Count == values.Length );

            for (int i = 0; i < values.Length; i++)
            {
                object value = values[ i ];
                ITextDataConverter converter = this.converters[ i ];
                TextDataColumn column = this.columns[ i ];
                string valueString = converter.ToString( value, column );
                Contract.Assert( !string.IsNullOrEmpty( valueString ) );
                Contract.Assert( column.MaxLength == valueString.Length );
                this.textWriter.Write( valueString );
            }
        }
    }
}