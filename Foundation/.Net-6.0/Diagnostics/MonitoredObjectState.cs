using System;

namespace Foundation.Diagnostics;

internal sealed class MonitoredObjectState
{
    public readonly MonitoredObject MonitoredObject;
    private readonly long _timestamp;

    public MonitoredObjectState(MonitoredObject monitoredObject, long timestamp)
    {
        MonitoredObject = monitoredObject;
        _timestamp = timestamp;
    }

    public int? GetGeneration()
    {
        var generation = GetGeneration(MonitoredObject.WeakReference);
        return generation;
    }

    public long GetAge() => _timestamp - MonitoredObject.Timestamp;

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