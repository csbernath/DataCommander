using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Foundation.Collections;
using Foundation.Text;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [Pure]
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? Enumerable.Empty<TSource>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [Pure]
        public static IEnumerable<PreviousAndCurrent<TSource>> SelectPreviousAndCurrent<TSource>(this IEnumerable<TSource> source)
        {
            if (source != null)
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var previous = enumerator.Current;
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            yield return new PreviousAndCurrent<TSource>(previous, current);
                            previous = current;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        [Pure]
        public static IEnumerable<PreviousAndCurrent<TKey>> SelectPreviousAndCurrentKey<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);
#endif

            return source.Select(keySelector).SelectPreviousAndCurrent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <param name="partitionCount"></param>
        /// <returns></returns>
        public static IEnumerable<List<TSource>> GetPartitions<TSource>(
            this IEnumerable<TSource> source,
            int count,
            int partitionCount)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(partitionCount > 0);

            Contract.Ensures(Contract.Result<IEnumerable<List<TSource>>>().Count() <= partitionCount);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<List<TSource>>>().ToList(), partition => partition.Count > 0));
#endif

            var partitionSize = count/partitionCount;
            var remainder = count%partitionCount;

            using (var enumerator = source.GetEnumerator())
            {
                for (var partitionIndex = 0; partitionIndex < partitionCount; partitionIndex++)
                {
                    var currentPartitionSize = partitionSize;
                    if (remainder > 0)
                    {
                        currentPartitionSize++;
                        remainder--;
                    }

                    if (currentPartitionSize > 0)
                    {
                        var partition = enumerator.Take(currentPartitionSize);
                        if (partition.Count > 0)
                        {
                            yield return partition;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source)
        {
            return source.OrderBy(IdentityFunction<TSource>.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="isSeparator"></param>
        /// <returns></returns>
        public static IEnumerable<TSource[]> Split<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> isSeparator)
        {
            var list = new List<TSource>();
            foreach (var item in source)
            {
                if (isSeparator(item))
                {
                    yield return list.ToArray();
                    list = new List<TSource>();
                }
                else
                {
                    list.Add(item);
                }
            }

            if (list.Count > 0)
            {
                yield return list.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="initialSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static DynamicArray<TSource> ToDynamicArray<TSource>(this IEnumerable<TSource> source, int initialSize, int maxSize)
        {
            var dynamicArray = new DynamicArray<TSource>(initialSize, maxSize);
            dynamicArray.Add(source);
            return dynamicArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static string ToLogString<TSource>(this IEnumerable<TSource> source, Func<TSource, string> toString)
        {
            var sb = new StringBuilder();
            var index = 0;
            foreach (var item in source)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendFormat("[{0}] = {1}", index, toString(item));
                index++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="segmentSize"></param>
        /// <returns></returns>
        public static SegmentedCollection<TSource> ToSegmentedCollection<TSource>(this IEnumerable<TSource> source, int segmentSize)
        {
            var collection = new SegmentedCollection<TSource>(segmentSize);
            collection.Add(source);
            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);
#endif

            var dictionary = new SortedDictionary<TKey, TValue>();
            dictionary.Add(source, keySelector);
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source, IComparer<T> comparer)
        {
            return new SortedSet<T>(source, comparer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static string ToString<T>(this IEnumerable<T> source, string separator, Func<T, string> toString)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(toString != null);
#endif

            string result;
            if (source != null)
            {
                var sb = new StringBuilder();
                foreach (var item in source)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }

                    var itemString = toString(item);
                    sb.Append(itemString);
                }

                result = sb.ToString();
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static string ToString<TSource>(this IEnumerable<TSource> source, IReadOnlyCollection<StringTableColumnInfo<TSource>> columns)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(columns != null);
#endif

            var table = new StringTable(columns.Count);

#region First row: column names

            var row = table.NewRow();
            var columnIndex = 0;
            foreach (var column in columns)
            {
                row[columnIndex] = column.ColumnName;
                table.Columns[columnIndex].Align = column.Align;
                ++columnIndex;
            }
            table.Rows.Add(row);

#endregion

#region Second row: underline first row

            var secondRow = table.NewRow();
            table.Rows.Add(secondRow);

#endregion

            foreach (var item in source)
            {
                row = table.NewRow();
                columnIndex = 0;
                foreach (var column in columns)
                {
                    row[columnIndex] = column.ToStringFunction(item);
                    ++columnIndex;
                }
                table.Rows.Add(row);
            }

#region Fill second row

            var columnWidths = new int[columns.Count];
            columnIndex = 0;
            foreach (var column in columns)
            {
                var max = table.Rows.Select(r => r[columnIndex] == null ? 0 : r[columnIndex].Length).Max();
                secondRow[columnIndex] = new string('-', max);
                columnWidths[columnIndex] = max;
                ++columnIndex;
            }

#endregion

            return table.ToString(columnWidths, " ");
        }
    }
}