using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Linq
{
    public static class IEnumerableExtensions
    {
        public static bool CountIsGreaterThan<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, int count)
        {
            Assert.IsNotNull(source);

            var countIsGreaterThan = false;
            var filteredCount = 0;

            foreach (var item in source)
            {
                if (predicate(item))
                {
                    ++filteredCount;
                    if (filteredCount > count)
                    {
                        countIsGreaterThan = true;
                        break;
                    }
                }
            }

            return countIsGreaterThan;
        }

        [Pure]
        public static IEnumerable<TSource> EmptyIfNull<TSource>(this IEnumerable<TSource> source) => source ?? Enumerable.Empty<TSource>();

        public static IEnumerable<List<TSource>> GetPartitions<TSource>(this IEnumerable<TSource> source, int count, int partitionCount)
        {
            Assert.IsNotNull(source);
            FoundationContract.Requires<ArgumentOutOfRangeException>(count >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(partitionCount > 0);

            FoundationContract.Ensures(Contract.Result<IEnumerable<List<TSource>>>().Count() <= partitionCount);
            FoundationContract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<List<TSource>>>().ToList(), partition => partition.Count > 0));

            var partitionSize = count / partitionCount;
            var remainder = count % partitionCount;

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
                            yield return partition;
                        else
                            break;
                    }
                    else
                        break;
                }
            }
        }

        public static LinerSearchResult<TSource, TResult> LinearSearch<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            Func<TResult, bool> breaker,
            Func<TResult, TResult, bool> comparer)
        {
            var selectedIndex = -1;
            var selectedSource = default(TSource);
            var selectedResult = default(TResult);
            var currentIndex = 0;

            foreach (var currentSource in source)
            {
                if (currentIndex == 0)
                {
                    selectedIndex = currentIndex;
                    selectedSource = currentSource;
                    selectedResult = selector(currentSource);
                }
                else
                {
                    var currentResult = selector(currentSource);
                    if (breaker(currentResult))
                        break;

                    var comparisonResult = comparer(currentResult, selectedResult);
                    if (comparisonResult)
                    {
                        selectedIndex = currentIndex;
                        selectedSource = currentSource;
                        selectedResult = currentResult;
                    }
                }

                ++currentIndex;
            }

            return new LinerSearchResult<TSource, TResult>(selectedIndex, selectedSource, selectedResult);
        }

        public static string ToString<T>(this IEnumerable<T> source, string separator, Func<T, string> toString)
        {
            Assert.IsNotNull(toString);

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
                result = null;

            return result;
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

        public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source) => source.OrderBy(IdentityFunction<TSource>.Instance);

        [Pure]
        public static IEnumerable<TResult> SelectWhere<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> select,
            Func<TResult, bool> where)
        {
            foreach (var item in source)
            {
                var result = select(item);
                if (where(result))
                    yield return result;
            }
        }

        [Pure]
        public static IEnumerable<TResult> SelectWhereIsNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> select)
        {
            foreach (var item in source)
            {
                var result = select(item);
                if (result != null)
                    yield return result;
            }
        }

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
                    list.Add(item);
            }

            if (list.Count > 0)
                yield return list.ToArray();
        }

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

        public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
        {
            Assert.IsNotNull(source);
            return new ReadOnlyCollection<TSource>(source.ToList());
        }

        public static ReadOnlyDictionary<TKey, TSource> ToReadOnlyDictionary<TKey, TSource>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var dictionary = source.ToDictionary(keySelector);
            return new ReadOnlyDictionary<TKey, TSource>(dictionary);
        }

        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(keySelector);

            var dictionary = new SortedDictionary<TKey, TValue>();
            dictionary.Add(source, keySelector);
            return dictionary;
        }

        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> source, IComparer<T> comparer) => new SortedSet<T>(source, comparer);
    }
}