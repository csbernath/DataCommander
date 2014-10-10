namespace DataCommander.Foundation.Data
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataRow
    {
        private readonly TextDataColumnCollection columns;
        private readonly Convert convert;
        private readonly Object[] values;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="convert"></param>
        public TextDataRow(TextDataColumnCollection columns, Convert convert)
        {
            Contract.Requires(columns != null);
            Contract.Requires(convert != null);

            this.columns = columns;
            this.convert = convert;
            this.values = new Object[columns.Count];

            for (Int32 i = 0; i < this.values.Length; i++)
            {
                this.values[i] = DBNull.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public delegate Object Convert(Object value, TextDataColumn column);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Object this[String columnName]
        {
            get
            {
                Int32 index = this.columns.IndexOf(columnName, true);
                return this.values[index];
            }

            set
            {
                Int32 index = this.columns.IndexOf(columnName, true);
                TextDataColumn column = this.columns[index];
                Object convertedValue = this.convert(value, column);
                this.values[index] = convertedValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Object[] ItemArray
        {
            get
            {
                return this.values;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns
        {
            get
            {
                return this.columns;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public Object this[TextDataColumn column]
        {
            get
            {
                Int32 index = this.columns.IndexOf(column, true);
                return this.values[index];
            }

            set
            {
                Int32 index = this.columns.IndexOf(column, true);
                Object convertedValue = this.convert(value, column);
                this.values[index] = convertedValue;
            }
        }
    }
}