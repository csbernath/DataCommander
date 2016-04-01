namespace DataCommander.Foundation.Text
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class StringTableColumnInfo<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="align"></param>
        /// <param name="toStringFunction"></param>
        public StringTableColumnInfo(
            string columnName,
            StringTableColumnAlign align,
            Func<T, string> toStringFunction)
        {
            Contract.Requires<ArgumentNullException>(toStringFunction != null);

            this.ColumnName = columnName;
            this.Align = align;
            this.ToStringFunction = toStringFunction;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 
        /// </summary>
        public StringTableColumnAlign Align { get; }

        /// <summary>
        /// 
        /// </summary>
        public Func<T, string> ToStringFunction { get; }
    }
}