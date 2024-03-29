﻿namespace Foundation.Collections.IndexableCollection;

public struct GetKeyResponse<T>(bool hasKey, T key)
{
    public bool HasKey { get; } = hasKey;

    public T Key { get; } = key;
}

public static class GetKeyResponse
{
    public static GetKeyResponse<T> Create<T>(bool hasKey, T key)
    {
        return new GetKeyResponse<T>(hasKey, key);
    }
}