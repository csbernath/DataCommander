#if FOUNDATION_3_5 || FOUNDATION_4_0

namespace DataCommander.Foundation.Collections
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue this[ TKey key ]
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<TKey> Keys
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<TValue> Values
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey( TKey key );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue( TKey key, out TValue value );
    }
}

#endif