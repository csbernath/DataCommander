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
            _fullFileName = fileName;
            _shortFileName = ShortFileName;
            var fileInfo = new FileInfo(fileName);
            var path = fileInfo.DirectoryName;
            _fileName = fileInfo.Name.ToUpper(CultureInfo.InvariantCulture);
            _watcher = new System.IO.FileSystemWatcher(path);
            _watcher.Changed += OnChanged;
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 
        /// </summary>
        public NotifyFilters NotifyFilter
        {
            get => _watcher.NotifyFilter;

            set => _watcher.NotifyFilter = value;
        }

        /// <summary>
        /// <see cref="System.IO.FileSystemWatcher.EnableRaisingEvents"/>
        /// </summary>
        public bool EnableRaisingEvents
        {
            get => _watcher.EnableRaisingEvents;

            set => _watcher.EnableRaisingEvents = value;
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
                if (_shortFileName == null)
                {
                    var sb = new StringBuilder(255);
                    var i = NativeMethods.GetShortPathName(_fullFileName, sb, (uint) sb.Capacity);

                    if (i > 0)
                    {
                        _shortFileName = sb.ToString().ToUpper(CultureInfo.InvariantCulture);
                        var fileInfo = new FileInfo(_shortFileName);
                        _shortFileName = fileInfo.Name;
                    }
                }

                return _shortFileName;
            }
        }

        private void TimerCallback(object state)
        {
            Log.Trace("Calling FileSystemWatcher.Changed event handlers... count: " + _count);
            Changed(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, _fullFileName, _fullFileName));
            _count = 0;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (Changed != null)
            {
                var name = e.Name;

                if (name == _fileName || name == ShortFileName)
                {
                    ////byte[] hash = ComputeHash(fullFileName);
                    ////bool changed = Compare(this.hash, hash) != 0;

                    ////if (changed)
                    ////{
                    ////    this.hash = hash;
                    ////    Changed(sender, e);
                    ////}

                    lock (_timer)
                    {
                        if (_count == 0)
                        {
                            _timer.Change(10000, Timeout.Infinite);
                        }

                        _count++;
                    }

                    Log.Trace("FileSystemWatcher.OnChanged: {0},{1}", name, e.ChangeType);
                }
            }
        }
    }
}