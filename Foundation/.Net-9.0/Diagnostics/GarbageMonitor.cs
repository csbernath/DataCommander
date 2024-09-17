using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Foundation.Collections;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Diagnostics;

public sealed class GarbageMonitor(string garbageMonitorName)
{
    private static readonly StringTableColumnInfo<MonitoredObjectState>[] Columns;
    public static readonly GarbageMonitor Default = new("Default");

    private readonly LinkedList<MonitoredObject> _monitoredObjects = [];
    private readonly InterlockedSequence _interlockedSequence = new(0);

    static GarbageMonitor()
    {
        Columns =
        [
            StringTableColumnInfo.CreateRight<MonitoredObjectState, long>("Id", i => i.MonitoredObject.Id),
            StringTableColumnInfo.CreateLeft<MonitoredObjectState>("Name", i => i.MonitoredObject.Name),
            StringTableColumnInfo.CreateLeft<MonitoredObjectState>("TypeName", i => i.MonitoredObject.TypeName),
            StringTableColumnInfo.CreateRight<MonitoredObjectState, int>("Size", i => i.MonitoredObject.Size),
            StringTableColumnInfo.CreateRight<MonitoredObjectState>("Time",
                i => i.MonitoredObject.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)),
            StringTableColumnInfo.CreateRight<MonitoredObjectState>("DisposeTime",
                i => i.MonitoredObject.DisposeTime?.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)),
            StringTableColumnInfo.CreateRight<MonitoredObjectState>("Age", i => StopwatchTimeSpan.ToString(i.GetAge(), 3)),
            StringTableColumnInfo.CreateLeft<MonitoredObjectState, bool>("IsAlive", i => i.MonitoredObject.WeakReference.IsAlive),
            StringTableColumnInfo.CreateRight<MonitoredObjectState, int?>("Generation", i => i.GetGeneration())
        ];
    }

    public int Count => _monitoredObjects.Count;

    public string State
    {
        get
        {
            string state;

            lock (_monitoredObjects)
            {
                var timestamp = Stopwatch.GetTimestamp();
                var listItemStates = _monitoredObjects.Select(i => new MonitoredObjectState(i, timestamp));
                var stringTable = listItemStates.ToString(Columns);
                var totalSize = _monitoredObjects.Sum(s => s.Size);
                state = $"GarbageMonitor.State:\r\nid: {garbageMonitorName}r\ntotalSize: {totalSize}\r\n{stringTable}";
            }

            MonitorExtensions.TryLock(_monitoredObjects, RemoveGarbageCollectedObjects);

            return state;
        }
    }

    public void Add(string name, object target)
    {
        ArgumentNullException.ThrowIfNull(target);
        var size = 0;

        var type = target.GetType();
        var typeName = TypeNameCollection.GetTypeName(type);

        if (target is string targetString)
        {
            var length = targetString.Length;
            size = length << 1;
        }

        Add(name, typeName, size, target);
    }

    public void Add(string name, string typeName, int size, object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        var id = _interlockedSequence.Next();

        var time = LocalTime.Default.Now;
        var timestamp = Stopwatch.GetTimestamp();
        var weakReference = new WeakReference(target);
        var monitoredObject = new MonitoredObject(id, name, typeName, size, time, timestamp, weakReference);

        lock (_monitoredObjects)
            _monitoredObjects.AddLast(monitoredObject);
    }

    public void SetDisposeTime(object target, DateTime disposeTime)
    {
        ArgumentNullException.ThrowIfNull(target);

        lock (_monitoredObjects)
        {
            var item = _monitoredObjects.First(i => i.WeakReference.Target == target);
            item.SetDisposeTime(disposeTime);
        }
    }

    private void RemoveGarbageCollectedObjects()
    {
        var node = _monitoredObjects.First;

        while (node != null)
        {
            var item = node.Value;
            var nextNode = node.Next;

            if (!item.WeakReference.IsAlive)
                _monitoredObjects.Remove(node);

            node = nextNode;
        }
    }
}