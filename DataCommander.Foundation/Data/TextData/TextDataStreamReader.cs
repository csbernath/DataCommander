namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataStreamReader
    {
        #region Private Fields

        private readonly TextReader textReader;

        private IList<TextDataColumn> columns;

        private IList<ITextDataConverter> converters;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="columns"></param>
        /// <param name="converters"></param>
        public TextDataStreamReader( TextReader textReader, IList<TextDataColumn> columns, IList<ITextDataConverter> converters )
        {
            Contract.Requires( textReader != null );
            Contract.Requires( columns != null );
            Contract.Requires( converters != null );

            this.textReader = textReader;
            this.columns = columns;
            this.converters = converters;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Object[] ReadRow()
        {
            Object[] values = null;
            Int32 index = 0;

            foreach (TextDataColumn column in this.columns)
            {
                Int32 maxLength = column.MaxLength;
                Char[] buffer = new Char[maxLength];
                Int32 count = this.textReader.Read( buffer, 0, maxLength );

                if (count == 0)
                {
                    break;
                }

                Contract.Assert( count == maxLength );

                if (index == 0)
                {
                    values = new Object[this.columns.Count];
                }

                String source = new String( buffer );
                ITextDataConverter converter = this.converters[ index ];
                Contract.Assert( converter != null );
                Object value;

                try
                {
                    value = converter.FromString( source, column );
                }
                catch (Exception e)
                {
                    throw new TextDataFormatException( column, converter, source, e );
                }

                values[ index ] = value;
                index++;
            }

            return values;
        }
    }
}