namespace Foundation.Collections.IndexableCollection;

public readonly struct GetKeyResponse<T>(bool hasKey, T key)
{
    public readonly bool HasKey = hasKey;
    public readonly T Key = key;
}

public static class GetKeyResponse
{
    public static GetKeyResponse<T> Create<T>(bool hasKey, T key) => new(hasKey, key);
}