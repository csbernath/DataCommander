#if FOUNDATION_3_5
namespace Foundation.Collections
{
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProducerConsumerCollection<T> : ICollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool TryAdd( T item );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool TryTake( out T item );
    }
}
#endif