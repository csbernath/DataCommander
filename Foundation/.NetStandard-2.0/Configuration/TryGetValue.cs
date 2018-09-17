namespace Foundation.Configuration
{
    public delegate bool TryGetValue<in TKey, TValue>(TKey key, out TValue value);
}