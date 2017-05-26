using System;

namespace Foundation.Text
{
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
                var valueString = value != null ? value.ToString() : null;
                return valueString;
            });
        }
    }
}