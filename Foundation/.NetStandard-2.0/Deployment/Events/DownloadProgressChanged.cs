using System.Net;

namespace Foundation.Deployment.Events
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