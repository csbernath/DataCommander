using System;
using System.Collections;

namespace Foundation.Collections
{
    internal sealed class EmptyNonGenericEnumerator : IEnumerator
    {
        public static readonly EmptyNonGenericEnumerator Value = new();

        private EmptyNonGenericEnumerator()
        {
        }

        public bool MoveNext() => false;

        public void Reset()
        {
        }

        public object Current => throw new InvalidOperationException();
    }
}