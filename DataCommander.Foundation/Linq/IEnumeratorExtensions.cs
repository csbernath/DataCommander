namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class IEnumeratorExtensions
    {
        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
        {
            Contract.Requires<ArgumentNullException>(enumerator != null);

            return new Enumerable<T>(enumerator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> Take<T>(this IEnumerator<T> enumerator, int count)
        {
            Contract.Requires<ArgumentNullException>(enumerator != null);
            Contract.Requires<ArgumentOutOfRangeException>(count >= 0);

            var list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                if (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    list.Add(item);
                }
                else
                {
                    break;
                }
            }

            return list;
        }

        #endregion

        #region Private Classes

        private sealed class Enumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerator<T> enumerator;

            public Enumerable(IEnumerator<T> enumerator)
            {
                Contract.Requires(enumerator != null);

                this.enumerator = enumerator;
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.enumerator;
            }
        }

        #endregion
    }
}