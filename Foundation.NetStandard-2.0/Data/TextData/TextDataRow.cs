using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data.TextData
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataRow
    {
        private readonly Convert _convert;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="convert"></param>
        public TextDataRow(TextDataColumnCollection columns, Convert convert)
        {
            FoundationContract.Requires<ArgumentException>(columns != null);
            FoundationContract.Requires<ArgumentException>(convert != null);

            Columns = columns;
            _convert = convert;
            ItemArray = new object[columns.Count];

            for (var i = 0; i < ItemArray.Length; i++)
            {
                ItemArray[i] = DBNull.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public delegate object Convert(object value, TextDataColumn column);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public object this[string columnName]
        {
            get
            {
                var index = Columns.IndexOf(columnName, true);
                return ItemArray[index];
            }

            set
            {
                var index = Columns.IndexOf(columnName, true);
                var column = Columns[index];
                var convertedValue = _convert(value, column);
                ItemArray[index] = convertedValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object[] ItemArray { get; }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public object this[TextDataColumn column]
        {
            get
            {
                var index = Columns.IndexOf(column, true);
                return ItemArray[index];
            }

            set
            {
                var index = Columns.IndexOf(column, true);
                var convertedValue = _convert(value, column);
                ItemArray[index] = convertedValue;
            }
        }
    }
}