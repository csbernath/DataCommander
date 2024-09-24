namespace Foundation.Collections.IndexableCollection;

public readonly struct GetKeyResponse<T>(bool hasKey, T key)
{
    public bool HasKey { get; } = hasKey;

    public T Key { get; } = key;
}

public static class GetKeyResponse
{
    public static GetKeyResponse<T> Create<T>(bool hasKey, T key) => new GetKeyResponse<T>(hasKey, key);
}