namespace Foundation.Configuration
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate Boolean TryGetValue<TKey, TValue>(TKey key, out TValue value);
}