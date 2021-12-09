using System;
using System.Globalization;
using System.IO;
using Foundation.Log;

namespace Foundation.Threading;

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
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private readonly string _path;
    private readonly string _searchPattern;
    private readonly int _period;
    private string[] _last;

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
        int period)
    {
        _path = path;
        _searchPattern = searchPattern;
        _period = period;

        Initialize(this);
        var name = string.Format(CultureInfo.InvariantCulture, "FileSystemMonitor({0},{1})", path, searchPattern);
        Thread.Name = name;
    }

    /// <summary>
    /// Occurs when a file or directory in the specified path is created.
    /// </summary>
    public FileSystemEventHandler Created { get; set; }

    void ILoopable.First(Exception exception)
    {
    }

    void ILoopable.Next()
    {
        try
        {
            var current = Directory.GetFiles(_path, _searchPattern);
            Array.Sort(current);

            if (_last != null)
            {
                for (var i = 0; i < current.Length; i++)
                {
                    var file = current[i];
                    var index = Array.BinarySearch(_last, file);

                    if (index < 0 && Created != null)
                    {
                        var message = string.Format(CultureInfo.InvariantCulture,
                            "FileSystemMonitor({0}).Created: {1}", Thread.ManagedThreadId, file);
                        Log.Trace(message);

                        var fileName = Path.GetFileName(file);
                        var e = new FileSystemEventArgs(WatcherChangeTypes.Created, _path,
                            fileName);
                        Created(this, e);
                    }
                }

                for (var i = 0; i < _last.Length; i++)
                {
                    var file = _last[i];
                    var index = Array.BinarySearch(current, file);

                    if (index < 0)
                    {
                        Log.Trace("{0}.Deleted: {1}", Thread.Name, file);
                    }
                }
            }
            else
            {
                for (var i = 0; i < current.Length; i++)
                {
                    Log.Trace("FileSystemMonitor.current[{0}]: {1}", i, current[i]);
                }
            }

            _last = current;
        }
        catch (Exception e)
        {
            Log.Write(LogLevel.Error, e.ToString());
        }

        Thread.WaitForStop(_period);
    }

    void ILoopable.Last()
    {
    }
}