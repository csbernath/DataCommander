namespace Foundation.Collections.IndexableCollection;

public struct GetKeyResponse<T>
{
    public GetKeyResponse(bool hasKey, T key)
    {
        HasKey = hasKey;
        Key = key;
    }

    public bool HasKey { get; }

    public T Key { get; }
}

public static class GetKeyResponse
{
    public static GetKeyResponse<T> Create<T>(bool hasKey, T key)
    {
        return new GetKeyResponse<T>(hasKey, key);
    }
}