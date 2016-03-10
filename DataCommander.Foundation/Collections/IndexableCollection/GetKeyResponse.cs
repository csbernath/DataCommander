namespace DataCommander.Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct GetKeyResponse<T>
    {
        private readonly bool hasKey;
        private readonly T key;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasKey"></param>
        /// <param name="key"></param>
        public GetKeyResponse(bool hasKey, T key)
        {
            this.hasKey = hasKey;
            this.key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasKey => this.hasKey;

        /// <summary>
        /// 
        /// </summary>
        public T Key => this.key;
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