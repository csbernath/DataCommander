using System;
using System.Diagnostics;

namespace Foundation.Core
{
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    public struct NotNullable<T> where T : class
    {
        private readonly T _value;

        private NotNullable(T value)
        {
            if (value == null)
                throw new ArgumentNullException();

            _value = value;
        }

        public bool HasValue => _value != null;

        public T Value
        {
            get
            {
                if (_value == null)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        public static implicit operator NotNullable<T>(T value) => new(value);
        public static implicit operator T(NotNullable<T> notNullable) => notNullable.Value;
    }
}