namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

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
            var emptyArray = (IEnumerable<T>)EmptyArray<T>.Value;
            return emptyArray.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}