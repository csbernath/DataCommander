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

        private readonly IList<TextDataColumn> columns;

        private readonly IList<ITextDataConverter> converters;

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
        public object[] ReadRow()
        {
            object[] values = null;
            int index = 0;

            foreach (TextDataColumn column in this.columns)
            {
                int maxLength = column.MaxLength;
                Char[] buffer = new Char[maxLength];
                int count = this.textReader.Read( buffer, 0, maxLength );

                if (count == 0)
                {
                    break;
                }

                Contract.Assert( count == maxLength );

                if (index == 0)
                {
                    values = new object[this.columns.Count];
                }

                string source = new string( buffer );
                ITextDataConverter converter = this.converters[ index ];
                Contract.Assert( converter != null );
                object value;

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