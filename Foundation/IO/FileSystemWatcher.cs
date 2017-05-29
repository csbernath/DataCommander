using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Foundation.Log;

namespace Foundation.IO
{
    /// <summary>
    /// Encapsulates <see cref="System.IO.FileSystemWatcher"/> to minimize number of OnChange events
    /// and resolve short and long filename problem.
    /// </summary>
    public class FileSystemWatcher
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly System.IO.FileSystemWatcher _watcher;
        private readonly string _fullFileName;
        private readonly string _fileName;
        private string _shortFileName;
        private int _count;
        private readonly Timer _timer;

        /// <summary>
        /// Creates a new instance to watch NTFS events.
        /// </summary>
        /// <param name="fileName">The file to watch</param>
        public FileSystemWatcher(string fileName)
        {
            this._fullFileName = fileName;
            this._shortFileName = this.ShortFileName;
            var fileInfo = new FileInfo(fileName);
            var path = fileInfo.DirectoryName;
            this._fileName = fileInfo.Name.ToUpper(CultureInfo.InvariantCulture);
            this._watcher = new System.IO.FileSystemWatcher(path);
            this._watcher.Changed += this.OnChanged;
            this._timer = new Timer(this.TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        public NotifyFilters NotifyFilter
        {
            get => this._watcher.NotifyFilter;

            set => this._watcher.NotifyFilter = value;
        }

        /// <summary>
        /// <see cref="System.IO.FileSystemWatcher.EnableRaisingEvents"/>
        /// </summary>
        public bool EnableRaisingEvents
        {
            get => this._watcher.EnableRaisingEvents;

            set => this._watcher.EnableRaisingEvents = value;
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
                if (this._shortFileName == null)
                {
                    var sb = new StringBuilder(255);
                    var i = NativeMethods.GetShortPathName(this._fullFileName, sb, (uint) sb.Capacity);

                    if (i > 0)
                    {
                        this._shortFileName = sb.ToString().ToUpper(CultureInfo.InvariantCulture);
                        var fileInfo = new FileInfo(this._shortFileName);
                        this._shortFileName = fileInfo.Name;
                    }
                }

                return this._shortFileName;
            }
        }

        private void TimerCallback(object state)
        {
            Log.Trace("Calling FileSystemWatcher.Changed event handlers... count: " + this._count);
            this.Changed(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, this._fullFileName, this._fullFileName));
            this._count = 0;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (this.Changed != null)
            {
                var name = e.Name;

                if (name == this._fileName || name == this.ShortFileName)
                {
                    ////byte[] hash = ComputeHash(fullFileName);
                    ////bool changed = Compare(this.hash, hash) != 0;

                    ////if (changed)
                    ////{
                    ////    this.hash = hash;
                    ////    Changed(sender, e);
                    ////}

                    lock (this._timer)
                    {
                        if (this._count == 0)
                        {
                            this._timer.Change(10000, Timeout.Infinite);
                        }

                        this._count++;
                    }

                    Log.Trace("FileSystemWatcher.OnChanged: {0},{1}", name, e.ChangeType);
                }
            }
        }
    }
}