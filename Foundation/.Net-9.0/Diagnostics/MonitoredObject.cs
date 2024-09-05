using System;

namespace Foundation.Diagnostics;

internal sealed class MonitoredObject(long id, string name, string typeName, int size, DateTime time, long timestamp, WeakReference weakReference)
{
    public readonly long Id = id;
    public readonly string Name = name;
    public readonly string TypeName = typeName;
    public readonly int Size = size;
    public readonly DateTime Time = time;
    public readonly long Timestamp = timestamp;
    public readonly WeakReference WeakReference = weakReference;
    private DateTime? _disposeTime;

    public DateTime? DisposeTime => _disposeTime;
    public void SetDisposeTime(DateTime dateTime) => _disposeTime = dateTime;
}