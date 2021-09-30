using System;

namespace Foundation.Text
{
    public static class StringTableColumnInfo
    {
        public static StringTableColumnInfo<TSource> Create<TSource, TResult>(string columnName, StringTableColumnAlign align, Func<TSource, TResult> getValue)
        {
            return new StringTableColumnInfo<TSource>(columnName, align, source =>
            {
                var value = getValue(source);
                var valueString = value?.ToString();
                return valueString;
            });
        }

        public static StringTableColumnInfo<TSource> CreateLeft<TSource>(string columnName, Func<TSource, string> toString) =>
            new(columnName, StringTableColumnAlign.Left, toString);

        public static StringTableColumnInfo<TSource> CreateLeft<TSource, TResult>(string columnName, Func<TSource, TResult> getValue) =>
            Create(columnName, StringTableColumnAlign.Left, getValue);

        public static StringTableColumnInfo<TSource> CreateRight<TSource>(string columnName, Func<TSource, string> toString) =>
            new(columnName, StringTableColumnAlign.Right, toString);

        public static StringTableColumnInfo<TSource> CreateRight<TSource, TResult>(string columnName, Func<TSource, TResult> getValue) =>
            Create(columnName, StringTableColumnAlign.Right, getValue);
    }
}