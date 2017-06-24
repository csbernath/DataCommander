using System;
using System.Collections.Generic;
using Foundation.Linq;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TypeDictionary<TValue>
    {
        private readonly Dictionary<Type, TValue> _selections = new Dictionary<Type, TValue>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(TValue value)
        {
            var type = typeof (T);
            _selections.Add(type, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryGetValue<T>(out TValue value)
        {
            var type = typeof (T);
            return _selections.TryGetValue(type, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TValue GetValueOrDefault(Type type)
        {
            return _selections.GetValueOrDefault(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TValue GetValueOrDefault<T>()
        {
            var type = typeof (T);
            return GetValueOrDefault(type);
        }
    }
}