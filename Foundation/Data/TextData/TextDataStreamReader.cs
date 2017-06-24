using System;
using System.Collections.Generic;
using System.IO;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataStreamReader
    {
        #region Private Fields

        private readonly TextReader _textReader;

        private readonly IList<TextDataColumn> _columns;

        private readonly IList<ITextDataConverter> _converters;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="columns"></param>
        /// <param name="converters"></param>
        public TextDataStreamReader(TextReader textReader, IList<TextDataColumn> columns, IList<ITextDataConverter> converters)
        {
#if CONTRACTS_FULL
            Contract.Requires(textReader != null);
            Contract.Requires(columns != null);
            Contract.Requires(converters != null);
#endif

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

#if CONTRACTS_FULL
                Contract.Assert(count == maxLength);
#endif

                if (index == 0)
                {
                    values = new object[_columns.Count];
                }

                var source = new string(buffer);
                var converter = _converters[index];
#if CONTRACTS_FULL
                Contract.Assert(converter != null);
#endif
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
}