namespace DataCommander.Foundation.Collections
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct GetKeyResponse<T>
    {
        private readonly Boolean hasKey;
        private readonly T key;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hasKey"></param>
        /// <param name="key"></param>
        public GetKeyResponse( Boolean hasKey, T key )
        {
            this.hasKey = hasKey;
            this.key = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean HasKey
        {
            get
            {
                return this.hasKey;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Key
        {
            get
            {
                return this.key;
            }
        }
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
        public static GetKeyResponse<T> Create<T>( Boolean hasKey, T key )
        {
            return new GetKeyResponse<T>( hasKey, key );
        }
    }
}