using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foundation.Collections.ReadOnly;

public static class IListExtensions
{
    extension<T>(IList<T> list)
    {
        public ReadOnlyCollection<T> ToReadOnlyCollection()
        {
            ArgumentNullException.ThrowIfNull(list);

            var readOnlyCollection = list.Count == 0
                ? EmptyReadOnlyCollection<T>.Value
                : new ReadOnlyCollection<T>(list);

            return readOnlyCollection;
        }
    }
}