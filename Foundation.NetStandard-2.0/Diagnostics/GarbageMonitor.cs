using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Foundation.Configuration;
using Foundation.Diagnostics.Assertions;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// Summary description for GcMonitor.
    /// </summary>
    public static class GarbageMonitor
    {
        #region Private Fields

        private static readonly LinkedList<ListItem> items = new LinkedList<ListItem>();
        private static long id;
        private static readonly StringTableColumnInfo<ListItemState>[] columns;

        #endregion

        static GarbageMonitor()
        {
            columns = new[]
            {
                StringTableColumnInfo.Create<ListItemState, long>("Id", StringTableColumnAlign.Right, i => i.ListItem.Id),
                new StringTableColumnInfo<ListItemState>("Name", StringTableColumnAlign.Left, i => i.ListItem.Name),
                new StringTableColumnInfo<ListItemState>("TypeName", StringTableColumnAlign.Left, i => i.ListItem.TypeName),
                StringTableColumnInfo.Create<ListItemState, int>("Size", StringTableColumnAlign.Right, i => i.ListItem.Size),
                new StringTableColumnInfo<ListItemState>("Time", StringTableColumnAlign.Right,
                    i => i.ListItem.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                new StringTableColumnInfo<ListItemState>("DisposeTime", StringTableColumnAlign.Right,
                    i => i.ListItem.DisposeTime?.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                new StringTableColumnInfo<ListItemState>("Age", StringTableColumnAlign.Right, i => StopwatchTimeSpan.ToString(i.Age, 3)),
                StringTableColumnInfo.Create<ListItemState, bool>("IsAlive", StringTableColumnAlign.Left, i => i.ListItem.WeakReference.IsAlive),
                StringTableColumnInfo.Create<ListItemState, int?>("Generation", StringTableColumnAlign.Right, i => i.Generation)
            };
        }

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static int Count => items.Count;

        /// <summary>
        /// 
        /// </summary>
        public static string State
        {
            get
            {
                string state;

                lock (items)
                {
                    var timestamp = Stopwatch.GetTimestamp();
                    var listItemStates = items.Select(i => new ListItemState(i, timestamp)).ToList();
                    var stringTable = listItemStates.ToString(columns);
                    var totalSize = listItemStates.Sum(s => s.ListItem.Size);
                    state = string.Format(CultureInfo.InvariantCulture, "GarbageMonitor.State:\r\ntotalSize: {0}\r\n{1}", totalSize, stringTable);
                }

                var entered = Monitor.TryEnter(items);
                if (entered)
                {
                    RemoveGarbageCollectedItems();
                    Monitor.Exit(items);
                }

                return state;
            }
        }

        private static void RemoveGarbageCollectedItems()
        {
            var node = items.First;

            while (node != null)
            {
                var item = node.Value;
                var nextNode = node.Next;

                if (!item.WeakReference.IsAlive)
                    items.Remove(node);

                node = nextNode;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        public static void Add(string name, object target)
        {
            Assert.IsNotNull(target);

            string typeName = null;
            var size = 0;

            var type = target.GetType();
            typeName = TypeNameCollection.GetTypeName(type);

            var targetString = target as string;
            if (targetString != null)
            {
                var length = targetString.Length;
                size = length << 1;
            }

            Add(name, typeName, size, target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <param name="size"></param>
        /// <param name="target"></param>
        public static void Add(string name, string typeName, int size, object target)
        {
            Assert.IsNotNull(target);

            var id = Interlocked.Increment(ref GarbageMonitor.id);
            var item = new ListItem(id, name, typeName, size, target);

            lock (items)
            {
                items.AddLast(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="disposeTime"></param>
        public static void SetDisposeTime(object target, DateTime disposeTime)
        {
            Assert.IsNotNull(target);

            lock (items)
            {
                var item = items.FirstOrDefault(i => i.WeakReference.Target == target);
                if (item != null)
                {
                    item.DisposeTime = disposeTime;
                }
            }
        }

        #endregion

        private sealed class ListItem
        {
            public readonly long Id;
            public readonly string Name;
            public readonly string TypeName;
            public readonly int Size;
            public readonly DateTime Time = LocalTime.Default.Now;
            public readonly long Timestamp;
            public readonly WeakReference WeakReference;
            public DateTime? DisposeTime;

            public ListItem(long id, string name, string typeName, int size, object target)
            {
                Id = id;
                Name = name;
                TypeName = typeName;
                Size = size;
                WeakReference = new WeakReference(target);
                Timestamp = Stopwatch.GetTimestamp();
            }
        }

        private sealed class ListItemState
        {
            private readonly long _timestamp;

            public ListItemState(ListItem listItem, long timestamp)
            {
                ListItem = listItem;
                _timestamp = timestamp;

                if (listItem.WeakReference.IsAlive)
                {
                    try
                    {
                        Generation = GC.GetGeneration(ListItem.WeakReference);
                    }
                    catch
                    {
                    }
                }
            }

            public ListItem ListItem { get; }
            public int? Generation { get; }
            public long Age => _timestamp - ListItem.Timestamp;
        }
    }
}