namespace DataCommander.Foundation.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// Periodically checks a directory for file creations/deletions
    /// and raises events when a new file is created or a file is deleted.
    /// Can be used for monitoring network shares for new files.
    /// </summary>
    /// <remarks>
    /// <see cref="System.IO.FileSystemWatcher"/> is based on NTFS events which cannot be used on network shares.
    /// </remarks>
    public sealed class FileSystemMonitor : LoopThread, ILoopable
    {
        private static ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private string path;
        private string searchPattern;
        private int period;
        private string[] last;
        private FileSystemEventHandler created;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemMonitor"/> class.
        /// </summary>
        /// <param name="path">
        /// The directory to monitor, in standard or Universal Naming Convention (UNC) notation.
        /// </param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path.
        /// The parameter cannot end in two periods ("..") or contain two periods ("..")
        /// followed by DirectorySeparatorChar or AltDirectorySeparatorChar, nor can it contain any of the characters in InvalidPathChars. 
        /// </param>
        /// <param name="period">
        /// milliseconds
        /// </param>
        public FileSystemMonitor(
            string path,
            string searchPattern,
            int period )
        {
            this.path = path;
            this.searchPattern = searchPattern;
            this.period = period;

            this.Initialize( this );
            string name = string.Format( CultureInfo.InvariantCulture, "FileSystemMonitor({0},{1})", path, searchPattern );
            Thread.Name = name;
        }

        /// <summary>
        /// Occurs when a file or directory in the specified path is created.
        /// </summary>
        public FileSystemEventHandler Created
        {
            get
            {
                return this.created;
            }

            set
            {
                this.created = value;
            }
        }

        void ILoopable.First( Exception exception )
        {
        }

        void ILoopable.Next()
        {
            try
            {
                string[] current = Directory.GetFiles( this.path, this.searchPattern );
                Array.Sort( current );

                if (this.last != null)
                {
                    for (int i = 0; i < current.Length; i++)
                    {
                        string file = current[ i ];
                        int index = Array.BinarySearch( this.last, file );

                        if (index < 0 && this.Created != null)
                        {
                            string message = string.Format( CultureInfo.InvariantCulture, "FileSystemMonitor({0}).Created: {1}", this.Thread.ManagedThreadId, file );
                            log.Trace(message );

                            string fileName = Path.GetFileName( file );
                            FileSystemEventArgs e = new FileSystemEventArgs( WatcherChangeTypes.Created, this.path, fileName );
                            this.created( this, e );
                        }
                    }

                    for (int i = 0; i < this.last.Length; i++)
                    {
                        string file = this.last[ i ];
                        int index = Array.BinarySearch( current, file );

                        if (index < 0)
                        {
                            log.Trace("{0}.Deleted: {1}", Thread.Name, file );
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < current.Length; i++)
                    {
                        log.Trace("FileSystemMonitor.current[{0}]: {1}", i, current[ i ] );
                    }
                }

                this.last = current;
            }
            catch (Exception e)
            {
                log.Write( LogLevel.Error, e.ToString() );
            }

            this.Thread.WaitForStop( this.period );
        }

        void ILoopable.Last()
        {
        }
    }
}