using System;

namespace DataCommander.Update.Events
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