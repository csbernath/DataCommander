namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Text;

    /// <summary>
    /// 
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region Public Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [Pure]
        public static Boolean AllAreEqual<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            Boolean allAreEqual = true;

            if (source != null)
            {
                Boolean first = true;
                TSource firstItem = default(TSource);

                foreach (TSource item in source)
                {
                    if (first)
                    {
                        first = false;
                        firstItem = item;
                    }
                    else
                    {
                        if (!comparer.Equals(firstItem, item))
                        {
                            allAreEqual = false;
                            break;
                        }
                    }
                }
            }

            return allAreEqual;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerable enumerable)
        {
            foreach (TSource item in enumerable)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TResult> CreateList<TSource, TResult>(this IEnumerable<TSource> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var collection = source as ICollection<TSource>;
            List<TResult> list;

            if (collection != null)
            {
                Int32 count = collection.Count;

                if (count > 0)
                {
                    list = new List<TResult>(collection.Count);
                }
                else
                {
                    list = null;
                }
            }
            else
            {
                list = new List<TResult>();
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> source)
        {
            Contract.Requires(source != null);
            return source.Select(Clone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? Enumerable.Empty<TSource>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            Contract.Requires(action != null);

            if (source != null)
            {
                int index = 0;
                foreach (var item in source)
                {
                    action(item, index);
                    index++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="partitionSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> GetPartitions<T>(this IEnumerable<T> source, int partitionSize)
        {
            Contract.Requires(source != null);
            Contract.Requires(partitionSize > 0);

            var partition = new List<T>(partitionSize);

            foreach (var item in source)
            {
                if (partition.Count == partitionSize)
                {
                    yield return partition;
                    partition = new List<T>(partitionSize);
                }

                partition.Add(item);
            }

            if (partition.Count > 0)
            {
                yield return partition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IndexedItem<T> IndexOf<T>(this IEnumerable<T> source, Func<T, Boolean> predicate)
        {
            Contract.Requires(predicate != null);

            IndexedItem<T> result = null;
            if (source != null)
            {
                Int32 index = 0;
                foreach (var item in source)
                {
                    if (predicate(item))
                    {
                        result = new IndexedItem<T>(index, item);
                        break;
                    }

                    index++;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsNotNullAndAny<T>(this IEnumerable<T> source)
        {
            Boolean any;

            if (source != null)
            {
                var collection = source as ICollection<T>;
                if (collection != null)
                {
                    any = collection.Count > 0;
                }
                else
                {
                    var readOnlyCollection = source as IReadOnlyCollection<T>;
                    if (readOnlyCollection != null)
                    {
                        any = readOnlyCollection.Count > 0;
                    }
                    else
                    {
                        any = source.Any();
                    }
                }
            }
            else
            {
                any = false;
            }

            return any;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Boolean IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static MinMaxResult<TSource> MinMax<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Boolean> where,
            Func<TSource, TResult> select,
            IComparer<TResult> comparer)
        {
            MinMaxResult<TSource> minMaxResult;
            if (source != null)
            {
                Contract.Assert(select != null);

                Int32 count = 0;
                Int32 whereCount = 0;
                Int32 minIndex = -1;
                TSource minItem = default(TSource);
                TResult minSelected = default(TResult);
                Int32 maxIndex = -1;
                TSource maxItem = default(TSource);
                TResult maxSelected = default(TResult);

                foreach (var currentItem in source)
                {
                    if (where == null || where(currentItem))
                    {
                        var currentSelected = select(currentItem);
                        if (minIndex == -1 || comparer.Compare(currentSelected, minSelected) < 0)
                        {
                            minIndex = count;
                            minItem = currentItem;
                            minSelected = currentSelected;
                        }

                        if (maxIndex == -1 || comparer.Compare(currentSelected, maxSelected) > 0)
                        {
                            maxIndex = count;
                            maxItem = currentItem;
                            maxSelected = currentSelected;
                        }

                        whereCount++;
                    }

                    count++;
                }

                minMaxResult = new MinMaxResult<TSource>(
                    count,
                    whereCount,
                    minIndex >= 0 ? new IndexedItem<TSource>(minIndex, minItem) : null,
                    maxIndex >= 0 ? new IndexedItem<TSource>(maxIndex, maxItem) : null);
            }
            else
            {
                minMaxResult = new MinMaxResult<TSource>(0, 0, null, null);
            }

            return minMaxResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static MinMaxResult<TSource> MinMax<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Boolean> where,
            Func<TSource, TResult> select) where TResult : IComparable<TResult>
        {
            return source.MinMax(where, select, Comparer<TResult>.Default);
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
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<IndexedItem<TSource>> SelectIndexedItem<TSource>(this IEnumerable<TSource> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            int index = 0;

            foreach (var item in source)
            {
                yield return new IndexedItem<TSource>(index, item);
                index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="isSeparator"></param>
        /// <returns></returns>
        public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source, Func<T, bool> isSeparator)
        {
            var list = new List<T>();
            foreach (var item in source)
            {
                if (isSeparator(item))
                {
                    yield return list.ToArray();
                    list = new List<T>();
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
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="source"></param>
        /// <param name="createCollection"></param>
        /// <returns></returns>
        public static TCollection ToCollection<TSource, TCollection>(
            this IEnumerable<TSource> source,
            Func<TCollection> createCollection) where TCollection : ICollection<TSource>
        {
            Contract.Requires(createCollection != null);

            TCollection collection = default(TCollection);
            if (source != null)
            {
                collection = createCollection();
                collection.Add(source);
            }
            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="source"></param>
        /// <param name="createCollection"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public static TCollection ToCollection<TSource, TCollection>(
            this IEnumerable<TSource> source,
            Func<TCollection> createCollection,
            Action<TCollection, TSource> add)
        {
            Contract.Requires(createCollection != null);
            Contract.Requires(add != null);

            TCollection collection = default(TCollection);
            if (source != null)
            {
                collection = createCollection();
                foreach (var item in source)
                {
                    add(collection, item);
                }
            }
            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            dictionary.Add(source);
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="initialSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static DynamicArray<TSource> ToDynamicArray<TSource>(this IEnumerable<TSource> source, Int32 initialSize, Int32 maxSize)
        {
            var dynamicArray = new DynamicArray<TSource>(initialSize, maxSize);
            dynamicArray.Add(source);
            return dynamicArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static String ToLogString<T>(this IEnumerable<T> source, Func<T, String> toString)
        {
            var sb = new StringBuilder();
            Int32 index = 0;
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
        public static SegmentCollection<TSource> ToSegmentCollection<TSource>(this IEnumerable<TSource> source, Int32 segmentSize)
        {
            var segmentedList = new SegmentCollection<TSource>(segmentSize);
            segmentedList.Add(source);
            return segmentedList;
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
            var dictionary = new SortedDictionary<TKey, TValue>();
            dictionary.Add(source, keySelector);
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="toString"></param>
        /// <returns></returns>
        public static String ToString<T>(this IEnumerable<T> source, String separator, Func<T, String> toString)
        {
            Contract.Requires(toString != null);

            String result;
            if (source != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in source)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(separator);
                    }

                    String itemString = toString(item);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static StringTable ToStringTable<T>(this IEnumerable<T> source, params StringTableColumnInfo<T>[] columns)
        {
            var table = new StringTable(columns.Length);

            #region First row: column names

            var row = table.NewRow();
            for (Int32 i = 0; i < columns.Length; i++)
            {
                row[i] = columns[i].ColumnName;
                table.Columns[i].Align = columns[i].Align;
            }
            table.Rows.Add(row);

            #endregion

            Int32 rowIndex = 0;
            foreach (T item in source)
            {
                row = table.NewRow();
                for (Int32 i = 0; i < columns.Length; i++)
                {
                    row[i] = columns[i].ToString(item, rowIndex);
                }
                table.Rows.Add(row);
                rowIndex++;
            }

            #region Second row: underline first row

            row = table.NewRow();
            for (Int32 i = 0; i < columns.Length; i++)
            {
                Int32 max = table.Rows.Select(r => r[i] == null ? 0 : r[i].Length).Max();
                row[i] = new String('-', max);
            }
            table.Rows.Insert(1, row);

            #endregion

            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Where<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, Int32, Boolean> where,
            Func<TSource, Int32, TResult> select)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(where != null);
            Contract.Requires<ArgumentNullException>(select != null);

            Int32 index = 0;

            foreach (var item in source)
            {
                if (where(item, index))
                {
                    yield return select(item, index);
                }

                index++;
            }
        }

        #endregion

        #region Private Static Methods

        private static T Clone<T>(T source)
        {
            var cloneable = (ICloneable)source;
            Object cloneObject = cloneable.Clone();
            T clone = (T)cloneObject;
            return clone;
        }

        #endregion
    }
}