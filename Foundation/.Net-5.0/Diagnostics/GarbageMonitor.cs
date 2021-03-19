using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Foundation.Assertions;
using Foundation.Collections;
using Foundation.Core;
using Foundation.Text;

namespace Foundation.Diagnostics
{
    public sealed class GarbageMonitor
    {
        private static readonly StringTableColumnInfo<MonitoredObjectState>[] Columns;
        public static readonly GarbageMonitor Default = new GarbageMonitor("Default");

        private readonly string _garbageMonitorName;
        private readonly LinkedList<MonitoredObject> _monitoredObjects = new LinkedList<MonitoredObject>();
        private readonly InterlockedSequence _interlockedSequence = new InterlockedSequence(0);

        static GarbageMonitor()
        {
            Columns = new[]
            {
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
            };
        }

        public GarbageMonitor(string garbageMonitorName) => _garbageMonitorName = garbageMonitorName;

        #region Public Properties

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
                    state = $"GarbageMonitor.State:\r\nid: {_garbageMonitorName}r\ntotalSize: {totalSize}\r\n{stringTable}";
                }

                MonitorExtensions.TryLock(_monitoredObjects, RemoveGarbageCollectedObjects);

                return state;
            }
        }

        #endregion

        #region Public Methods

        public void Add(string name, object target)
        {
            Assert.IsNotNull(target);

            string typeName = null;
            var size = 0;

            var type = target.GetType();
            typeName = TypeNameCollection.GetTypeName(type);

            if (target is string targetString)
            {
                var length = targetString.Length;
                size = length << 1;
            }

            Add(name, typeName, size, target);
        }

        public void Add(string name, string typeName, int size, object target)
        {
            Assert.IsNotNull(target);

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
            Assert.IsNotNull(target);

            lock (_monitoredObjects)
            {
                var item = _monitoredObjects.First(i => i.WeakReference.Target == target);
                item.SetDisposeTime(disposeTime);
            }
        }

        #endregion

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
}