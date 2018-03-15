using System.Collections.Generic;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollectionIndex<T> : ICollection<T>
    {
        /// <summary>
        /// 
        /// </summary>
        string Name
        {
            get;
        }
    }
}