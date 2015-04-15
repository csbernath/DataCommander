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
        private static readonly Lazy<StringTableColumnInfo<ListItemState>[]> columns;

        #endregion

        static GarbageMonitor()
        {
            columns = new Lazy<StringTableColumnInfo<ListItemState>[]>( () =>
                new[]
                {
                    new StringTableColumnInfo<ListItemState>("Id", StringTableColumnAlign.Right, i=>i.ListItem.Id),
                    new StringTableColumnInfo<ListItemState>("Name", StringTableColumnAlign.Left, i=>i.ListItem.Name),
                    new StringTableColumnInfo<ListItemState>("TypeName", StringTableColumnAlign.Left, i=>i.ListItem.TypeName),
                    new StringTableColumnInfo<ListItemState>("Size", StringTableColumnAlign.Right, i=>i.ListItem.Size),
                    new StringTableColumnInfo<ListItemState>("Time", StringTableColumnAlign.Right, i=>i.ListItem.Time.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                    new StringTableColumnInfo<ListItemState>("DisposeTime", StringTableColumnAlign.Right, i=>ToString(i.ListItem.DisposeTime)),
                    new StringTableColumnInfo<ListItemState>("Age", StringTableColumnAlign.Right, i=>StopwatchTimeSpan.ToString(i.Age,3)),
                    new StringTableColumnInfo<ListItemState>("IsAlive", StringTableColumnAlign.Left, i=>i.ListItem.WeakReference.IsAlive),
                    new StringTableColumnInfo<ListItemState>("Generation", StringTableColumnAlign.Right, i=>ToString(i.Generation))
                } );
        }

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public static int Count
        {
            get
            {
                return items.Count;
            }
        }

        private static string ToString( int? source )
        {
            return source != null ? source.Value.ToString() : null;
        }

        private static string ToString( DateTime? source )
        {
            return source != null ? source.Value.ToString( "HH:mm:ss.fff", CultureInfo.InvariantCulture ) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string State
        {
            get
            {
                StringTable stringTable;
                int totalSize = 0;

                lock (items)
                {
                    long timestamp = Stopwatch.GetTimestamp();
                    var listItemStates = items.Select( i => new ListItemState( i, timestamp ) ).ToArray();
                    stringTable = listItemStates.ToStringTable( columns.Value );
                    totalSize = listItemStates.Sum( s => s.ListItem.Size );

                    var removeableItems =
                        from listItemState in listItemStates
                        where !listItemState.IsAlive
                        select listItemState.ListItem;

                    items.Remove( removeableItems );
                }

                return string.Format( CultureInfo.InvariantCulture, "GarbageMonitor.State:\r\ntotalSize: {0}\r\n{1}", totalSize, stringTable );
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        public static void Add( string name, object target )
        {
            Contract.Requires<ArgumentNullException>(target != null);

            string typeName = null;
            int size = 0;

            if (target != null)
            {
                Type type = target.GetType();
                typeName = TypeNameCollection.GetTypeName( type );

                if (type == typeof( string ))
                {
                    string s = (string) target;
                    int length = s.Length;
                    size = length << 1;
                }
            }

            Add( name, typeName, size, target );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeName"></param>
        /// <param name="size"></param>
        /// <param name="target"></param>
        public static void Add(
            string name,
            string typeName,
            int size,
            object target )
        {
            Contract.Requires<ArgumentNullException>(target != null);

            long id = Interlocked.Increment( ref GarbageMonitor.id );
            ListItem item = new ListItem( id, name, typeName, size, target );

            lock (items)
            {
                items.AddLast( item );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="disposeTime"></param>
        public static void SetDisposeTime( object target, DateTime disposeTime )
        {
            Contract.Requires( target != null );

            lock (items)
            {
                var item = items.FirstOrDefault( i => i.WeakReference.Target == target );
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

            private readonly long id;
            private readonly string name;
            private readonly string typeName;
            private readonly int size;
            private readonly DateTime time = LocalTime.Default.Now;
            private readonly long timestamp;
            private readonly WeakReference weakReference;
            private DateTime? disposeTime;

            #endregion

            public ListItem(
                long id,
                string name,
                string typeName,
                int size,
                object target )
            {
                this.id = id;
                this.name = name;
                this.typeName = typeName;
                this.size = size;
                this.weakReference = new WeakReference( target );
                this.timestamp = Stopwatch.GetTimestamp();
            }

            public long Id
            {
                get
                {
                    return this.id;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public string TypeName
            {
                get
                {
                    return this.typeName;
                }
            }

            public int Size
            {
                get
                {
                    return this.size;
                }
            }

            public DateTime Time
            {
                get
                {
                    return this.time;
                }
            }

            public long Timestamp
            {
                get
                {
                    return this.timestamp;
                }
            }

            public WeakReference WeakReference
            {
                get
                {
                    return this.weakReference;
                }
            }

            public DateTime? DisposeTime
            {
                get
                {
                    return this.disposeTime;
                }
                set
                {
                    this.disposeTime = value;
                }
            }
        }

        private sealed class ListItemState
        {
            private readonly ListItem listItem;
            private readonly long timestamp;
            private readonly bool isAlive;
            private readonly int? generation;

            public ListItemState( ListItem listItem, long timestamp )
            {
                this.listItem = listItem;
                this.timestamp = timestamp;

                this.isAlive = listItem.WeakReference.IsAlive;
                if (this.isAlive)
                {
                    try
                    {
                        this.generation = GC.GetGeneration( this.listItem.WeakReference );
                    }
                    catch
                    {
                        this.isAlive = false;
                    }
                }
            }

            public ListItem ListItem
            {
                get
                {
                    return this.listItem;
                }
            }

            public bool IsAlive
            {
                get
                {
                    return this.isAlive;
                }
            }

            public int? Generation
            {
                get
                {
                    return this.generation;
                }
            }

            public long Age
            {
                get
                {
                    return this.timestamp - this.listItem.Timestamp;
                }
            }
        }
    }
}