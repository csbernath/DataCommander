using System.Collections;
using System.Collections.Generic;

namespace Foundation.Collections
{
    public sealed class EmptyEnumerable<T> : IEnumerable<T>
    {
        public static readonly EmptyEnumerable<T> Value = new();

        private EmptyEnumerable()
        {
        }

        public IEnumerator<T> GetEnumerator() => EmptyEnumerator<T>.Value;
        IEnumerator IEnumerable.GetEnumerator() => EmptyNonGenericEnumerator.Value;
    }
}