namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TypeDictionary<TValue>
    {
        private readonly Dictionary<Type, TValue> selections = new Dictionary<Type, TValue>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(TValue value)
        {
            var type = typeof (T);
            this.selections.Add(type, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TryGetValue<T>(out TValue value)
        {
            var type = typeof (T);
            return this.selections.TryGetValue(type, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TValue GetValueOrDefault(Type type)
        {
            return this.selections.GetValueOrDefault(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public TValue GetValueOrDefault<T>()
        {
            var type = typeof (T);
            return this.GetValueOrDefault(type);
        }
    }
}