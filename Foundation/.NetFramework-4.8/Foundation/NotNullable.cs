using System;
using System.Diagnostics;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        /// <summary>
        /// 
        /// </summary>
        public bool HasValue => _value != null;

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                if (_value == null)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator NotNullable<T>(T value)
        {
            return new NotNullable<T>(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notNullable"></param>
        /// <returns></returns>
        public static implicit operator T(NotNullable<T> notNullable)
        {
            return notNullable.Value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class NotNullableExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NotNullable<T> ToNotNullable<T>(this T value) where T : class
        {
            return value;
        }
    }
}