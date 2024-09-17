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
                long timestamp = Stopwatch.GetTimestamp();
                IEnumerable<MonitoredObjectState> listItemStates = _monitoredObjects.Select(i => new MonitoredObjectState(i, timestamp));
                string stringTable = listItemStates.ToString(Columns);
                int totalSize = _monitoredObjects.Sum(s => s.Size);
                state = $"GarbageMonitor.State:\r\nid: {garbageMonitorName}r\ntotalSize: {totalSize}\r\n{stringTable}";
            }

            MonitorExtensions.TryLock(_monitoredObjects, RemoveGarbageCollectedObjects);

            return state;
        }
    }

    public void Add(string name, object target)
    {
        ArgumentNullException.ThrowIfNull(target);
        int size = 0;

        Type type = target.GetType();
        string typeName = TypeNameCollection.GetTypeName(type);

        if (target is string targetString)
        {
            int length = targetString.Length;
            size = length << 1;
        }

        Add(name, typeName, size, target);
    }

    public void Add(string name, string typeName, int size, object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        long id = _interlockedSequence.Next();

        DateTime time = LocalTime.Default.Now;
        long timestamp = Stopwatch.GetTimestamp();
        WeakReference weakReference = new WeakReference(target);
        MonitoredObject monitoredObject = new MonitoredObject(id, name, typeName, size, time, timestamp, weakReference);

        lock (_monitoredObjects)
            _monitoredObjects.AddLast(monitoredObject);
    }

    public void SetDisposeTime(object target, DateTime disposeTime)
    {
        ArgumentNullException.ThrowIfNull(target);

        lock (_monitoredObjects)
        {
            MonitoredObject item = _monitoredObjects.First(i => i.WeakReference.Target == target);
            item.SetDisposeTime(disposeTime);
        }
    }

    private void RemoveGarbageCollectedObjects()
    {
        LinkedListNode<MonitoredObject> node = _monitoredObjects.First;

        while (node != null)
        {
            MonitoredObject item = node.Value;
            LinkedListNode<MonitoredObject> nextNode = node.Next;

            if (!item.WeakReference.IsAlive)
                _monitoredObjects.Remove(node);

            node = nextNode;
        }
    }
}