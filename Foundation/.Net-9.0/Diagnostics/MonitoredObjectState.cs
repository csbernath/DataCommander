using System;

namespace Foundation.Diagnostics;

internal sealed class MonitoredObjectState(MonitoredObject monitoredObject, long timestamp)
{
    public readonly MonitoredObject MonitoredObject = monitoredObject;

    public int? GetGeneration()
    {
        int? generation = GetGeneration(MonitoredObject.WeakReference);
        return generation;
    }

    public long GetAge() => timestamp - MonitoredObject.Timestamp;

    private static int? GetGeneration(WeakReference weakReference)
    {
        int? generation = null;
        if (weakReference.IsAlive)
        {
            try
            {
                generation = GC.GetGeneration(weakReference);
            }
            catch
            {
            }
        }

        return generation;
    }
}