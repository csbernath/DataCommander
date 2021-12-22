using System;

namespace Foundation.Diagnostics;

internal sealed class MonitoredObject
{
    public readonly long Id;
    public readonly string Name;
    public readonly string TypeName;
    public readonly int Size;
    public readonly DateTime Time;
    public readonly long Timestamp;
    public readonly WeakReference WeakReference;
    private DateTime? _disposeTime;

    public MonitoredObject(long id, string name, string typeName, int size, DateTime time, long timestamp, WeakReference weakReference)
    {
        Id = id;
        Name = name;
        TypeName = typeName;
        Size = size;
        Time = time;
        Timestamp = timestamp;
        WeakReference = weakReference;
    }

    public DateTime? DisposeTime => _disposeTime;
    public void SetDisposeTime(DateTime dateTime) => _disposeTime = dateTime;
}