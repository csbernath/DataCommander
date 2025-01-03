using System.Net;

namespace Foundation.Deployment.Events;

public sealed class DownloadProgressChanged(DownloadProgressChangedEventArgs downloadProgressChangedEventArgs) : Event
{
    public readonly DownloadProgressChangedEventArgs DownloadProgressChangedEventArgs = downloadProgressChangedEventArgs;
}