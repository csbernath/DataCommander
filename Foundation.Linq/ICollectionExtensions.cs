using System.Collections.Generic;

namespace Foundation.Linq
{
    public static class ICollectionExtensions
    {
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            //FoundationContract.Requires<ArgumentException>(collection != null || items == null);

            if (items != null)
                foreach (var item in items)
                    collection.Add(item);
        }
    }
}