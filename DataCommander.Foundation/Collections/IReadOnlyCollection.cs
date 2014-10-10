#if FOUNDATION_3_5 || FOUNDATION_4_0

namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        /// <summary>
        /// 
        /// </summary>
        Int32 Count { get; }
    }
}

#endif