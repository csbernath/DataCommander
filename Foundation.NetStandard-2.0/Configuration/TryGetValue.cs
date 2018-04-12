using System;

namespace Foundation.Configuration
{
    public delegate Boolean TryGetValue<in TKey, TValue>(TKey key, out TValue value);
}