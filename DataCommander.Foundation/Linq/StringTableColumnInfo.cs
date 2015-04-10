namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StringTableColumnInfo<T>
    {
        private readonly string columnName;
        private readonly StringTableColumnAlign align;
        private readonly Func<T, int, string> toString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="align"></param>
        /// <param name="toString"></param>
        public StringTableColumnInfo(
            string columnName,
            StringTableColumnAlign align,
            Func<T, int, string> toString)
        {
            Contract.Requires<ArgumentNullException>(toString != null);

            this.columnName = columnName;
            this.align = align;
            this.toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="align"></param>
        /// <param name="getValue"></param>
        public StringTableColumnInfo(
            string columnName,
            StringTableColumnAlign align,
            Func<T, object> getValue)
        {
            Contract.Requires<ArgumentNullException>(getValue != null);

            this.columnName = columnName;
            this.align = align;
            this.toString = delegate(T item, int index)
            {
                object value = getValue(item);
                return ToString(value);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnAlign Align
        {
            get
            {
                return this.align;
            }
        }

        internal string ToString(T item, int index)
        {
            return this.toString(item, index);
        }

        private static string ToString(object source)
        {
            string result;
            if (source != null)
            {
                result = source.ToString();
            }
            else
            {
                result = "null";
            }
            return result;
        }
    }
}