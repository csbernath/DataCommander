namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class EmptyReadOnlyList<T> : IReadOnlyList<T>
    {
        private static readonly EmptyReadOnlyList<T> instance = new EmptyReadOnlyList<T>();

        private EmptyReadOnlyList()
        {
        }

        public static IReadOnlyList<T> Instance
        {
            get
            {
                return instance;
            }
        }

        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                return 0;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}