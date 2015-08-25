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
        public static bool AllAreEqual<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            bool allAreEqual = true;

            if (source != null)
            {
                bool first = true;
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
        /// <param name="source"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Pure]
        public static IReadOnlyList<TSource> AsReadOnlyList<TSource>(this IEnumerable<TSource> source)
        {
            IReadOnlyList<TSource> readOnlyList = null;

            Selection.CreateArgumentIsSelection(source)
                .IfArgumentIsNull(delegate
                {
                    readOnlyList = EmptyReadOnlyList<TSource>.Instance;
                })
                .IfArgumentIs(delegate(IList<TSource> list)
                {
                    readOnlyList = list.AsReadOnlyList();
                })
                .IfArgumentIs(delegate(IReadOnlyList<TSource> list)
                {
                    readOnlyList = list;
                })
                .Else(delegate
                {
                    throw new ArgumentException();
                });

            return readOnlyList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [Pure]
        public static List<TResult> CreateList<TSource, TResult>(this IEnumerable<TSource> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var collection = source as ICollection<TSource>;
            List<TResult> list;

            if (collection != null)
            {
                int count = collection.Count;

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
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [Pure]
        public static IEnumerable<TSource> Clone<TSource>(this IEnumerable<TSource> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            return source.Select<TSource, TSource>(Clone);
        }

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
                            yield return new PreviousAndCurrent<TSource>(previous,current);
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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);

            return source.Select(keySelector).SelectPreviousAndCurrent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(partitionCount > 0);

            Contract.Ensures(Contract.Result<IEnumerable<List<TSource>>>().Count() <= partitionCount);
            Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<List<TSource>>>().ToList(), partition => partition.Count > 0));

            int partitionSize = count/partitionCount;
            int remainder = count%partitionCount;

            using (var enumerator = source.GetEnumerator())
            {
                for (int partitionIndex = 0; partitionIndex < partitionCount; partitionIndex++)
                {
                    int currentPartitionSize = partitionSize;
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
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IndexedItem<TSource> IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);

            int index = -1;
            TSource result = default(TSource);

            if (source != null)
            {
                foreach (var item in source)
                {
                    index++;
                    if (predicate(item))
                    {
                        result = item;
                        break;
                    }
                }
            }

            return new IndexedItem<TSource>(index, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNotNullAndAny<TSource>(this IEnumerable<TSource> source)
        {
            bool any;

            if (source != null)
            {
                var collection = source as ICollection<TSource>;
                if (collection != null)
                {
                    any = collection.Count > 0;
                }
                else
                {
                    var readOnlyCollection = source as IReadOnlyCollection<TSource>;
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
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <param name="select"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static MinMaxResult<TSource> MinMax<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, bool> where,
            Func<TSource, TResult> select,
            IComparer<TResult> comparer)
        {
            MinMaxResult<TSource> minMaxResult;
            if (source != null)
            {
                Contract.Assert(select != null);

                int count = 0;
                int whereCount = 0;
                int minIndex = -1;
                TSource minItem = default(TSource);
                TResult minSelected = default(TResult);
                int maxIndex = -1;
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
                    new IndexedItem<TSource>(minIndex, minItem),
                    new IndexedItem<TSource>(maxIndex, maxItem));
            }
            else
            {
                minMaxResult = new MinMaxResult<TSource>(0, 0, new IndexedItem<TSource>(-1, default(TSource)), new IndexedItem<TSource>(-1, default(TSource)));
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
            Func<TSource, bool> where,
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
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="source"></param>
        /// <param name="createCollection"></param>
        /// <returns></returns>
        public static TCollection ToCollection<TSource, TCollection>(
            this IEnumerable<TSource> source,
            Func<TCollection> createCollection) where TCollection : ICollection<TSource>
        {
            Contract.Requires<ArgumentNullException>(createCollection != null);

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
            Contract.Requires<ArgumentNullException>(createCollection != null);
            Contract.Requires<ArgumentNullException>(add != null);

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
            int index = 0;
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
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(keySelector != null);

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
            Contract.Requires<ArgumentNullException>(toString != null);

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

                    string itemString = toString(item);
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
        public static StringTable ToStringTable<TSource>(this IEnumerable<TSource> source, params StringTableColumnInfo<TSource>[] columns)
        {
            var table = new StringTable(columns.Length);

            #region First row: column names

            var row = table.NewRow();
            for (int i = 0; i < columns.Length; i++)
            {
                row[i] = columns[i].ColumnName;
                table.Columns[i].Align = columns[i].Align;
            }
            table.Rows.Add(row);

            #endregion

            int rowIndex = 0;
            foreach (TSource item in source)
            {
                row = table.NewRow();
                for (int i = 0; i < columns.Length; i++)
                {
                    row[i] = columns[i].ToString(item, rowIndex);
                }
                table.Rows.Add(row);
                rowIndex++;
            }

            #region Second row: underline first row

            row = table.NewRow();
            for (int i = 0; i < columns.Length; i++)
            {
                int max = table.Rows.Select(r => r[i] == null ? 0 : r[i].Length).Max();
                row[i] = new string('-', max);
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
            Func<TSource, int, bool> where,
            Func<TSource, int, TResult> select)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(where != null);
            Contract.Requires<ArgumentNullException>(select != null);

            int index = 0;

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
            object cloneObject = cloneable.Clone();
            T clone = (T)cloneObject;
            return clone;
        }

        #endregion
    }
}