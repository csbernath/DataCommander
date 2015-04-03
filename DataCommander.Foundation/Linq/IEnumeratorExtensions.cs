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
    }
}