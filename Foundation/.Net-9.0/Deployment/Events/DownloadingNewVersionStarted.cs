using System;

namespace Foundation.Deployment.Events;

public sealed class DownloadingNewVersionStarted(Version version) : Event
{
    public readonly Version Version = version;
}