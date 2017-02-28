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
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly System.IO.FileSystemWatcher watcher;
        private readonly string fullFileName;
        private readonly string fileName;
        private string shortFileName;
        private int count;
        private readonly Timer timer;

        /// <summary>
        /// Creates a new instance to watch NTFS events.
        /// </summary>
        /// <param name="fileName">The file to watch</param>
        public FileSystemWatcher(string fileName)
        {
            this.fullFileName = fileName;
            this.shortFileName = this.ShortFileName;
            var fileInfo = new FileInfo(fileName);
            var path = fileInfo.DirectoryName;
            this.fileName = fileInfo.Name.ToUpper(CultureInfo.InvariantCulture);
            this.watcher = new System.IO.FileSystemWatcher(path);
            this.watcher.Changed += this.OnChanged;
            this.timer = new Timer(this.TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
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
        public bool EnableRaisingEvents
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
        public FileSystemEventHandler Changed { get; set; }

        /// <summary>
        /// Gets the Int16 version of the fileName
        /// </summary>
        private string ShortFileName
        {
            get
            {
                if (this.shortFileName == null)
                {
                    var sb = new StringBuilder(255);
                    var i = NativeMethods.GetShortPathName(this.fullFileName, sb, (UInt32) sb.Capacity);

                    if (i > 0)
                    {
                        this.shortFileName = sb.ToString().ToUpper(CultureInfo.InvariantCulture);
                        var fileInfo = new FileInfo(this.shortFileName);
                        this.shortFileName = fileInfo.Name;
                    }
                }

                return this.shortFileName;
            }
        }

        private void TimerCallback(object state)
        {
            log.Trace("Calling FileSystemWatcher.Changed event handlers... count: " + this.count);
            this.Changed(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, this.fullFileName, this.fullFileName));
            this.count = 0;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (this.Changed != null)
            {
                var name = e.Name;

                if (name == this.fileName || name == this.ShortFileName)
                {
                    ////byte[] hash = ComputeHash(fullFileName);
                    ////bool changed = Compare(this.hash, hash) != 0;

                    ////if (changed)
                    ////{
                    ////    this.hash = hash;
                    ////    Changed(sender, e);
                    ////}

                    lock (this.timer)
                    {
                        if (this.count == 0)
                        {
                            this.timer.Change(10000, Timeout.Infinite);
                        }

                        this.count++;
                    }

                    log.Trace("FileSystemWatcher.OnChanged: {0},{1}", name, e.ChangeType);
                }
            }
        }
    }
}