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
            Contract.Requires(list != null);

            if (items != null)
            {
                foreach (Object item in items)
                {
                    list.Add(item);
                }
            }
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
                foreach (Object item in items)
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
            Contract.Requires(list != null);
            Contract.Requires(compareTo != null);

            IndexedItem<T> result = null;

            Int32 from = 0;
            Int32 to = list.Count - 1;

            while (from <= to)
            {
                Int32 index = from + (to - from)/2;
                var value = list[index];
                Int32 comparisonResult = compareTo(value);
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
        public static Int32 IndexOf<T>(this IList<T> source, Int32 startIndex, Func<T, Boolean> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);

            Int32 index = -1;
            if (source != null)
            {
                Int32 count = source.Count;
                for (Int32 i = startIndex; i < count; i++)
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
        public static Int32 LastIndexOf<T>(this IList<T> source, Int32 startIndex, Func<T, Boolean> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);
            Contract.Requires<ArgumentException>(startIndex >= 0);

            Int32 index = -1;
            if (source != null)
            {
                Contract.Assert(startIndex < source.Count);

                for (Int32 i = startIndex; i >= 0; i--)
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

            Int32 lastIndex = source.Count - 1;
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
        public static Int32 LastIndexOf<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Int32 index;
            if (source != null)
            {
                Int32 startIndex = source.Count - 1;
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
            private IList source;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            public CastedList(IList source)
            {
                Contract.Requires(source != null);

                this.source = source;
            }

            #region IList<TResult> Members

            Int32 IList<TResult>.IndexOf(TResult item)
            {
                return this.source.IndexOf(item);
            }

            void IList<TResult>.Insert(Int32 index, TResult item)
            {
                this.source.Insert(index, item);
            }

            void IList<TResult>.RemoveAt(Int32 index)
            {
                this.source.RemoveAt(index);
            }

            TResult IList<TResult>.this[Int32 index]
            {
                get
                {
                    Object valueObject = this.source[index];
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

            Boolean ICollection<TResult>.Contains(TResult item)
            {
                return this.source.Contains(item);
            }

            void ICollection<TResult>.CopyTo(TResult[] array, Int32 arrayIndex)
            {
                this.source.CopyTo(array, arrayIndex);
            }

            Int32 ICollection<TResult>.Count
            {
                get
                {
                    return this.source.Count;
                }
            }

            Boolean ICollection<TResult>.IsReadOnly
            {
                get
                {
                    return this.source.IsReadOnly;
                }
            }

            Boolean ICollection<TResult>.Remove(TResult item)
            {
                Int32 index = this.source.IndexOf(item);
                Boolean removed;

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
    }
}