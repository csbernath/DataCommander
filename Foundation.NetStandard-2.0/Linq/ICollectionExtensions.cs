using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Collections;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this ICollection collection)
        {
            var isNullOrEmpty = collection == null || collection.Count == 0;
            return isNullOrEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            var isNullOrEmpty = collection == null || collection.Count == 0;
            return isNullOrEmpty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {

            FoundationContract.Requires<ArgumentException>(collection != null || items == null);


            if (items != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static int Remove<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            FoundationContract.Requires<ArgumentException>(collection != null || items == null);

            var count = 0;

            if (items != null)
            {
                foreach (var item in items)
                {
                    var removed = collection.Remove(item);

                    if (removed)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static ICollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            FoundationContract.Requires<ArgumentNullException>(collection != null);

            return new ReadOnlyCollection<T>(collection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICollection<TResult> Cast<TResult>(this ICollection source)
        {
            ICollection<TResult> collection;

            if (source != null)
            {
                collection = new CastedCollection<TResult>(source);
            }
            else
            {
                collection = null;
            }

            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasElements<T>(this ICollection<T> source)
        {
            return source != null && source.Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(ICollection<T> source)
        {
            FoundationContract.Requires<ArgumentNullException>(source != null);

            var target = new T[source.Count];
            source.CopyTo(target, 0);
            return target;
        }

        public static ReadOnlyArray<T> ToReadOnlyArray<T>(this ICollection<T> source)
        {
            FoundationContract.Requires<ArgumentNullException>(source != null);

            var items = source.ToArray();
            return new ReadOnlyArray<T>(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        private sealed class CastedCollection<TResult> : ICollection<TResult>
        {
#region Private Fields

            /// <summary>
            /// 
            /// </summary>
            private readonly ICollection source;

            private readonly IList sourceAsList;

#endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            public CastedCollection(ICollection source)
            {
                FoundationContract.Requires<ArgumentNullException>(source != null);

                this.source = source;
                sourceAsList = source as IList;
            }

#region ICollection<TResult> Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            void ICollection<TResult>.Add(TResult item)
            {
                FoundationContract.Assert(sourceAsList != null);

                sourceAsList.Add(item);
            }

            /// <summary>
            /// 
            /// </summary>
            void ICollection<TResult>.Clear()
            {
                FoundationContract.Assert(sourceAsList != null);

                sourceAsList.Clear();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            bool ICollection<TResult>.Contains(TResult item)
            {
                bool contains;
                if (sourceAsList != null)
                {
                    contains = sourceAsList.Contains(item);
                }
                else
                {
                    var enumerable = (IEnumerable)source;
                    var enumerableT = enumerable.Cast<TResult>();
                    contains = enumerableT.Contains(item);
                }

                return contains;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            void ICollection<TResult>.CopyTo(TResult[] array, int arrayIndex)
            {
                source.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 
            /// </summary>
            int ICollection<TResult>.Count => source.Count;

            /// <summary>
            /// 
            /// </summary>
            bool ICollection<TResult>.IsReadOnly => throw new NotSupportedException();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            bool ICollection<TResult>.Remove(TResult item)
            {
                throw new NotImplementedException();
            }

#endregion

#region IEnumerable<TResult> Members

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerator<TResult> GetEnumerator()
            {
                foreach (TResult item in source)
                {
                    yield return item;
                }
            }

#endregion

#region IEnumerable Members

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

#endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class ReadOnlyCollection<T> : ICollection<T>
        {
            /// <summary>
            /// 
            /// </summary>
            private readonly ICollection<T> collection;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="collection"></param>
            public ReadOnlyCollection(ICollection<T> collection)
            {
                FoundationContract.Requires<ArgumentNullException>(collection != null);

                this.collection = collection;
            }

#region ICollection<T> Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            void ICollection<T>.Add(T item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// 
            /// </summary>
            void ICollection<T>.Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            bool ICollection<T>.Contains(T item)
            {
                FoundationContract.Ensures(!Contract.Result<bool>() || Count > 0);

                return collection.Contains(item);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                collection.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 
            /// </summary>
            public int Count => collection.Count;

            /// <summary>
            /// 
            /// </summary>
            public bool IsReadOnly => true;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException();
            }

#endregion

#region IEnumerable<T> Members

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return collection.GetEnumerator();
            }

#endregion

#region IEnumerable Members

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return collection.GetEnumerator();
            }

#endregion
        }
    }
}