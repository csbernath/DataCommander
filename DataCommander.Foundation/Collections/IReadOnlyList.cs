#if FOUNDATION_3_5 || FOUNDATION_4_0

namespace DataCommander.Foundation.Collections
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyList<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        T this[ int index ]
        {
            get;
        }
    }
}

#endif