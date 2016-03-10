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
        private readonly object[] values;

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
            this.values = new object[columns.Count];

            for (int i = 0; i < this.values.Length; i++)
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
                int index = this.columns.IndexOf(columnName, true);
                return this.values[index];
            }

            set
            {
                int index = this.columns.IndexOf(columnName, true);
                TextDataColumn column = this.columns[index];
                object convertedValue = this.convert(value, column);
                this.values[index] = convertedValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object[] ItemArray => this.values;

        /// <summary>
        /// 
        /// </summary>
        public TextDataColumnCollection Columns => this.columns;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public object this[TextDataColumn column]
        {
            get
            {
                int index = this.columns.IndexOf(column, true);
                return this.values[index];
            }

            set
            {
                int index = this.columns.IndexOf(column, true);
                object convertedValue = this.convert(value, column);
                this.values[index] = convertedValue;
            }
        }
    }
}