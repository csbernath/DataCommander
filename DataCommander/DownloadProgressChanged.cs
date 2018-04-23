using System.Net;

namespace DataCommander
{
    public sealed class DownloadProgressChanged : Event
    {
        public readonly DownloadProgressChangedEventArgs DownloadProgressChangedEventArgs;

        public DownloadProgressChanged(DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            DownloadProgressChangedEventArgs = downloadProgressChangedEventArgs;
        }
    }
}