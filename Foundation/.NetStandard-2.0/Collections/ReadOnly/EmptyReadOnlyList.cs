using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections.ReadOnly
{
    public sealed class EmptyReadOnlyList<T> : IReadOnlyList<T>
    {
        public static readonly EmptyReadOnlyList<T> Value = new EmptyReadOnlyList<T>();

        private EmptyReadOnlyList()
        {
        }

        T IReadOnlyList<T>.this[int index] => throw new ArgumentOutOfRangeException();
        public int Count => 0;

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyEnumerator<T>.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EmptyNonGenericEnumerator.Value;
        }
    }
}