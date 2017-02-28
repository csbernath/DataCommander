namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Text;

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
                new StringTableColumnInfo<ListItemState>("Time", StringTableColumnAlign.Right, i => i.ListItem.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)),
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
                {
                    items.Remove(node);
                }

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
            Contract.Requires<ArgumentNullException>(target != null);

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
            Contract.Requires<ArgumentNullException>(target != null);

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
            Contract.Requires<ArgumentNullException>(target != null);

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
            #region Private Fields

            #endregion

            public ListItem(
                long id,
                string name,
                string typeName,
                int size,
                object target)
            {
                this.Id = id;
                this.Name = name;
                this.TypeName = typeName;
                this.Size = size;
                this.WeakReference = new WeakReference(target);
                this.Timestamp = Stopwatch.GetTimestamp();
            }

            public long Id { get; }

            public string Name { get; }

            public string TypeName { get; }

            public int Size { get; }

            public DateTime Time { get; } = LocalTime.Default.Now;

            public long Timestamp { get; }

            public WeakReference WeakReference { get; }

            public DateTime? DisposeTime { get; set; }
        }

        private sealed class ListItemState
        {
            private readonly long timestamp;

            public ListItemState(ListItem listItem, long timestamp)
            {
                this.ListItem = listItem;
                this.timestamp = timestamp;

                this.IsAlive = listItem.WeakReference.IsAlive;
                if (this.IsAlive)
                {
                    try
                    {
                        this.Generation = GC.GetGeneration(this.ListItem.WeakReference);
                    }
                    catch
                    {
                        this.IsAlive = false;
                    }
                }
            }

            public ListItem ListItem { get; }

            public bool IsAlive { get; }

            public int? Generation { get; }

            public long Age => this.timestamp - this.ListItem.Timestamp;
        }
    }
}