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

        private static LinkedList<ListItem> items = new LinkedList<ListItem>();
        private static Int64 id;
        private static Lazy<StringTableColumnInfo<ListItemState>[]> columns;

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
        public static Int32 Count
        {
            get
            {
                return items.Count;
            }
        }

        private static string ToString( Int32? source )
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
        public static String State
        {
            get
            {
                StringTable stringTable;
                Int32 totalSize = 0;

                lock (items)
                {
                    Int64 timestamp = Stopwatch.GetTimestamp();
                    var listItemStates = items.Select( i => new ListItemState( i, timestamp ) ).ToArray();
                    stringTable = listItemStates.ToStringTable( columns.Value );
                    totalSize = listItemStates.Sum( s => s.ListItem.Size );

                    var removeableItems =
                        from listItemState in listItemStates
                        where !listItemState.IsAlive
                        select listItemState.ListItem;

                    items.Remove( removeableItems );
                }

                return String.Format( CultureInfo.InvariantCulture, "GarbageMonitor.State:\r\ntotalSize: {0}\r\n{1}", totalSize, stringTable );
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        public static void Add( String name, Object target )
        {
            Contract.Requires( target != null );

            String typeName = null;
            Int32 size = 0;

            if (target != null)
            {
                Type type = target.GetType();
                typeName = TypeNameCollection.GetTypeName( type );

                if (type == typeof( String ))
                {
                    String s = (String) target;
                    Int32 length = s.Length;
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
            String name,
            String typeName,
            Int32 size,
            Object target )
        {
            Contract.Requires( target != null );

            Int64 id = Interlocked.Increment( ref GarbageMonitor.id );
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
        public static void SetDisposeTime( Object target, DateTime disposeTime )
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

            private Int64 id;
            private String name;
            private String typeName;
            private Int32 size;
            private DateTime time = OptimizedDateTime.Now;
            private Int64 timestamp;
            private WeakReference weakReference;
            private DateTime? disposeTime;

            #endregion

            public ListItem(
                Int64 id,
                String name,
                String typeName,
                Int32 size,
                Object target )
            {
                this.id = id;
                this.name = name;
                this.typeName = typeName;
                this.size = size;
                this.weakReference = new WeakReference( target );
                this.timestamp = Stopwatch.GetTimestamp();
            }

            public Int64 Id
            {
                get
                {
                    return this.id;
                }
            }

            public String Name
            {
                get
                {
                    return this.name;
                }
            }

            public String TypeName
            {
                get
                {
                    return this.typeName;
                }
            }

            public Int32 Size
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

            public Int64 Timestamp
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
            private ListItem listItem;
            private Int64 timestamp;
            private bool isAlive;
            private Int32? generation;

            public ListItemState( ListItem listItem, Int64 timestamp )
            {
                this.listItem = listItem;
                this.timestamp = timestamp;

                this.isAlive = listItem.WeakReference.IsAlive;
                if (isAlive)
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

            public Boolean IsAlive
            {
                get
                {
                    return this.isAlive;
                }
            }

            public Int32? Generation
            {
                get
                {
                    return this.generation;
                }
            }

            public Int64 Age
            {
                get
                {
                    return this.timestamp - this.listItem.Timestamp;
                }
            }
        }
    }
}