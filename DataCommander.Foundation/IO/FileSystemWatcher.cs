namespace DataCommander.Foundation.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using DataCommander.Foundation.Diagnostics;

    /// <summary>
    /// Encapsulates <see cref="System.IO.FileSystemWatcher"/> to minimize number of OnChange events
    /// and resolve short and long filename problem.
    /// </summary>
    public class FileSystemWatcher
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private System.IO.FileSystemWatcher watcher;
        private String fullFileName;
        private String fileName;
        private String shortFileName;
        private Int32 count;
        private Timer timer;
        private FileSystemEventHandler changed;

        /// <summary>
        /// Creates a new instance to watch NTFS events.
        /// </summary>
        /// <param name="fileName">The file to watch</param>
        public FileSystemWatcher( String fileName )
        {
            this.fullFileName = fileName;
            this.shortFileName = this.ShortFileName;
            FileInfo fileInfo = new FileInfo( fileName );
            String path = fileInfo.DirectoryName;
            this.fileName = fileInfo.Name.ToUpper( CultureInfo.InvariantCulture );
            this.watcher = new System.IO.FileSystemWatcher( path );
            this.watcher.Changed += this.OnChanged;
            this.timer = new Timer( this.TimerCallback, null, Timeout.Infinite, Timeout.Infinite );
        }

        /// <summary>
        /// 
        /// </summary>
        public NotifyFilters NotifyFilter
        {
            get
            {
                return this.watcher.NotifyFilter;
            }

            set
            {
                this.watcher.NotifyFilter = value;
            }
        }

        /// <summary>
        /// <see cref="System.IO.FileSystemWatcher.EnableRaisingEvents"/>
        /// </summary>
        public Boolean EnableRaisingEvents
        {
            get
            {
                return this.watcher.EnableRaisingEvents;
            }

            set
            {
                this.watcher.EnableRaisingEvents = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileSystemEventHandler Changed
        {
            get
            {
                return this.changed;
            }

            set
            {
                this.changed = value;
            }
        }

        /// <summary>
        /// Gets the Int16 version of the fileName
        /// </summary>
        private String ShortFileName
        {
            get
            {
                if (this.shortFileName == null)
                {
                    var sb = new StringBuilder( 255 );
                    UInt32 i = NativeMethods.GetShortPathName( this.fullFileName, sb, (UInt32) sb.Capacity );

                    if (i > 0)
                    {
                        this.shortFileName = sb.ToString().ToUpper( CultureInfo.InvariantCulture );
                        FileInfo fileInfo = new FileInfo( this.shortFileName );
                        this.shortFileName = fileInfo.Name;
                    }
                }

                return this.shortFileName;
            }
        }

        private void TimerCallback( Object state )
        {
            log.Trace("Calling FileSystemWatcher.Changed event handlers... count: " + this.count );
            this.Changed( this, new FileSystemEventArgs( WatcherChangeTypes.Changed, this.fullFileName, this.fullFileName ) );
            this.count = 0;
        }

        private void OnChanged( Object sender, FileSystemEventArgs e )
        {
            if (this.Changed != null)
            {
                String name = e.Name;

                if (name == this.fileName || name == this.ShortFileName)
                {
                    ////Byte[] hash = ComputeHash(fullFileName);
                    ////Boolean changed = Compare(this.hash, hash) != 0;

                    ////if (changed)
                    ////{
                    ////    this.hash = hash;
                    ////    Changed(sender, e);
                    ////}

                    lock (this.timer)
                    {
                        if (this.count == 0)
                        {
                            this.timer.Change( 10000, Timeout.Infinite );
                        }

                        this.count++;
                    }

                    log.Trace("FileSystemWatcher.OnChanged: {0},{1}", name, e.ChangeType );
                }
            }
        }
    }
}