using System;
using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumeratorExtensions
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
            Assert.IsNotNull(enumerator);

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
            Assert.IsNotNull(enumerator);
            FoundationContract.Requires<ArgumentOutOfRangeException>(count >= 0);

            var list = new List<T>(count);

            for (var i = 0; i < count; i++)
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
            private readonly IEnumerator<T> _enumerator;

            public Enumerable(IEnumerator<T> enumerator)
            {
                Assert.IsNotNull(enumerator);

                this._enumerator = enumerator;
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return _enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _enumerator;
            }
        }

        #endregion
    }
}