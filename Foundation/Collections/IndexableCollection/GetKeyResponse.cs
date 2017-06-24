namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct GetKeyResponse<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasKey"></param>
        /// <param name="key"></param>
        public GetKeyResponse(bool hasKey, T key)
        {
            HasKey = hasKey;
            Key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasKey { get; }

        /// <summary>
        /// 
        /// </summary>
        public T Key { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class GetKeyResponse
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static GetKeyResponse<T> Create<T>(bool hasKey, T key)
        {
            return new GetKeyResponse<T>(hasKey, key);
        }
    }
}