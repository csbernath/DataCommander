namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="items"></param>
        public static void Add(this IList list, IEnumerable items)
        {
            Contract.Requires<ArgumentNullException>(list != null);

            if (items != null)
            {
                foreach (object item in items)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> source)
        {
            return new ReadOnlyListFromList<T>(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="items"></param>
        public static void Remove(this IList list, IEnumerable items)
        {
            Contract.Requires(items == null || list != null);

            if (items != null)
            {
                foreach (object item in items)
                {
                    list.Remove(item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static IndexedItem<T> BinarySearch<T>(this IList<T> list, Func<T, int> compareTo)
        {
            Contract.Requires<ArgumentNullException>(list != null);
            Contract.Requires<ArgumentNullException>(compareTo != null);

            IndexedItem<T> result = null;

            int from = 0;
            int to = list.Count - 1;

            while (from <= to)
            {
                int index = from + (to - from)/2;
                var value = list[index];
                int comparisonResult = compareTo(value);
                if (comparisonResult == 0)
                {
                    result = new IndexedItem<T>(index, value);
                    break;
                }
                else if (comparisonResult < 0)
                {
                    to = index - 1;
                }
                else
                {
                    from = index + 1;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<TResult> Cast<TResult>(this IList source)
        {
            IList<TResult> list;

            if (source != null)
            {
                list = new CastedList<TResult>(source);
            }
            else
            {
                list = null;
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IList<T> source, int startIndex, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);

            int index = -1;
            if (source != null)
            {
                int count = source.Count;
                for (int i = startIndex; i < count; i++)
                {
                    var item = source[i];
                    if (predicate(item))
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="startIndex"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this IList<T> source, int startIndex, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);
            Contract.Requires<ArgumentException>(startIndex >= 0);

            int index = -1;
            if (source != null)
            {
                Contract.Assert(startIndex < source.Count);

                for (int i = startIndex; i >= 0; i--)
                {
                    var item = source[i];
                    if (predicate(item))
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Last<T>(this IList<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentException>(source.Count > 0);

            int lastIndex = source.Count - 1;
            T last = source[lastIndex];
            return last;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int LastIndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            int index;
            if (source != null)
            {
                int startIndex = source.Count - 1;
                index = source.LastIndexOf(startIndex, predicate);
            }
            else
            {
                index = -1;
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private sealed class CastedList<TResult> : IList<TResult>
        {
            private readonly IList source;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            public CastedList(IList source)
            {
                Contract.Requires<ArgumentNullException>(source != null);

                this.source = source;
            }

            #region IList<TResult> Members

            int IList<TResult>.IndexOf(TResult item)
            {
                return this.source.IndexOf(item);
            }

            void IList<TResult>.Insert(int index, TResult item)
            {
                this.source.Insert(index, item);
            }

            void IList<TResult>.RemoveAt(int index)
            {
                this.source.RemoveAt(index);
            }

            TResult IList<TResult>.this[int index]
            {
                get
                {
                    object valueObject = this.source[index];
                    TResult value = (TResult) valueObject;
                    return value;
                }

                set
                {
                    this.source[index] = value;
                }
            }

            #endregion

            #region ICollection<TResult> Members

            void ICollection<TResult>.Add(TResult item)
            {
                this.source.Add(item);
            }

            void ICollection<TResult>.Clear()
            {
                this.source.Clear();
            }

            bool ICollection<TResult>.Contains(TResult item)
            {
                return this.source.Contains(item);
            }

            void ICollection<TResult>.CopyTo(TResult[] array, int arrayIndex)
            {
                this.source.CopyTo(array, arrayIndex);
            }

            int ICollection<TResult>.Count
            {
                get
                {
                    return this.source.Count;
                }
            }

            bool ICollection<TResult>.IsReadOnly
            {
                get
                {
                    return this.source.IsReadOnly;
                }
            }

            bool ICollection<TResult>.Remove(TResult item)
            {
                int index = this.source.IndexOf(item);
                bool removed;

                if (index >= 0)
                {
                    this.source.RemoveAt(index);
                    removed = true;
                }
                else
                {
                    removed = false;
                }

                return removed;
            }

            #endregion

            #region IEnumerable<TResult> Members

            IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator()
            {
                foreach (TResult item in this.source)
                {
                    yield return item;
                }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                IEnumerable<TResult> enumerable = this;
                return enumerable.GetEnumerator();
            }

            #endregion
        }

        private sealed class ReadOnlyListFromList<T> : IReadOnlyList<T>
        {
            private readonly IList<T> source;

            public ReadOnlyListFromList(IList<T> source)
            {
                Contract.Requires<ArgumentNullException>(source != null);

                this.source = source;
            }

            T IReadOnlyList<T>.this[int index]
            {
                get
                {
                    return this.source[index];
                }
            }

            int IReadOnlyCollection<T>.Count
            {
                get
                {
                    return this.source.Count;
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.source.GetEnumerator();
            }
        }
    }
}