namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{value}")]
    public struct NotNullable<T> where T : class
    {
        private readonly T value;

        private NotNullable(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasValue
        {
            get
            {
                return this.value != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                return this.value;
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