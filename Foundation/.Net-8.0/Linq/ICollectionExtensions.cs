using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Linq;

public static class ICollectionExtensions
{
    public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        //Assert.IsTrue(collection != null || items == null);

        if (items != null)
            foreach (var item in items)
                collection.Add(item);
    }

    public static ReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
    {
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));
        return new ReadOnlyCollection<T>(collection.ToList());
    }

    public static ICollection<TResult> Cast<TResult>(this ICollection source)
    {
        ICollection<TResult> collection = source != null ? new CastedCollection<TResult>(source) : null;
        return collection;
    }

    public static bool IsNullOrEmpty(this ICollection collection)
    {
        var isNullOrEmpty = collection == null || collection.Count == 0;
        return isNullOrEmpty;
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
    {
        var isNullOrEmpty = collection == null || collection.Count == 0;
        return isNullOrEmpty;
    }

    public static bool IsNullOrAny<T>(this ICollection<T> source) => source != null && source.Count > 0;

    public static int Remove<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        Assert.IsTrue(collection != null || items == null);

        var count = 0;

        if (items != null)
            foreach (var item in items)
            {
                var removed = collection.Remove(item);
                if (removed)
                    count++;
            }

        return count;
    }

    public static T[] ToArray<T>(ICollection<T> source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var target = new T[source.Count];
        source.CopyTo(target, 0);
        return target;
    }

    private sealed class CastedCollection<TResult> : ICollection<TResult>
    {
        #region Private Fields

        private readonly ICollection _source;
        private readonly IList _sourceAsList;

        #endregion

        public CastedCollection(ICollection source)
        {
            ArgumentNullException.ThrowIfNull(source);

            _source = source;
            _sourceAsList = source as IList;
        }

        #region ICollection<TResult> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<TResult>.Add(TResult item)
        {
            Assert.IsTrue(_sourceAsList != null);

            _sourceAsList.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<TResult>.Clear()
        {
            Assert.IsTrue(_sourceAsList != null);

            _sourceAsList.Clear();
        }

        bool ICollection<TResult>.Contains(TResult item)
        {
            bool contains;
            if (_sourceAsList != null)
            {
                contains = _sourceAsList.Contains(item);
            }
            else
            {
                var enumerable = (IEnumerable) _source;
                var enumerableT = enumerable.Cast<TResult>();
                contains = enumerableT.Contains(item);
            }

            return contains;
        }

        void ICollection<TResult>.CopyTo(TResult[] array, int arrayIndex) => _source.CopyTo(array, arrayIndex);
        int ICollection<TResult>.Count => _source.Count;
        bool ICollection<TResult>.IsReadOnly => throw new NotSupportedException();
        bool ICollection<TResult>.Remove(TResult item) => throw new NotImplementedException();

        #endregion

        #region IEnumerable<TResult> Members

        public IEnumerator<TResult> GetEnumerator()
        {
            foreach (TResult item in _source)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}