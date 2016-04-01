using System.Diagnostics;

namespace DataCommander.Foundation.Text
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class StringTableColumnInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="align"></param>
        /// <param name="getValue"></param>
        /// <returns></returns>
        public static StringTableColumnInfo<TSource> Create<TSource, TResult>(string columnName, StringTableColumnAlign align, Func<TSource, TResult> getValue)
        {
            return new StringTableColumnInfo<TSource>(columnName, align, source =>
            {
                var value = getValue(source);
                string valueString = value != null ? value.ToString() : null;
                return valueString;
            });
        }
    }
}