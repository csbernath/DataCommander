using System;

namespace Foundation.Deployment.Events
{
    public sealed class DownloadingNewVersionStarted : Event
    {
        public readonly Version Version;

        public DownloadingNewVersionStarted(Version version)
        {
            Version = version;
        }
    }
}