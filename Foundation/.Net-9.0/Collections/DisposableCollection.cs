using System;
using System.Collections.ObjectModel;

namespace Foundation.Collections;

public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable
{
    public void Dispose()
    {
        foreach (T item in this)
            if (item != null)
                item.Dispose();
    }
}