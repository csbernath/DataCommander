namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    internal sealed class TextDataStreamWriter
    {
        private readonly TextWriter textWriter;

        private readonly IList<ITextDataConverter> converters;

        public TextDataStreamWriter( TextWriter textWriter, IList<TextDataColumn> columns, IList<ITextDataConverter> converters )
        {
            Contract.Requires( textWriter != null );
            Contract.Requires( columns != null );
            Contract.Requires( converters != null );

            this.textWriter = textWriter;
            this.Columns = columns;
            this.converters = converters;
        }

        public IList<TextDataColumn> Columns { get; }

        public void WriteRow( object[] values )
        {
            Contract.Requires( values != null );
            Contract.Requires( this.Columns.Count == values.Length );

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[ i ];
                var converter = this.converters[ i ];
                var column = this.Columns[ i ];
                var valueString = converter.ToString( value, column );
                Contract.Assert( !string.IsNullOrEmpty( valueString ) );
                Contract.Assert( column.MaxLength == valueString.Length );
                this.textWriter.Write( valueString );
            }
        }
    }
}