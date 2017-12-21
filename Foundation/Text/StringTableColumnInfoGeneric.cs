using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Text
{
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
            FoundationContract.Requires<ArgumentNullException>(toStringFunction != null);

            ColumnName = columnName;
            Align = align;
            ToStringFunction = toStringFunction;
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