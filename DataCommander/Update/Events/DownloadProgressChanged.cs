using System.Net;

namespace DataCommander.Update.Events
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