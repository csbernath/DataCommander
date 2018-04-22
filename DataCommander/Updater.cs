using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Foundation;
using Foundation.Deployment;

namespace DataCommander
{
    public abstract class Event
    {
    }

    public sealed class CheckForUpdatesStarted : Event
    {
    }

    public sealed class DownloadingNewVersionStarted : Event
    {
    }

    public sealed class DownloadProgressChanged : Event
    {
        public readonly DownloadProgressChangedEventArgs DownloadProgressChangedEventArgs;

        public DownloadProgressChanged(DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            DownloadProgressChangedEventArgs = downloadProgressChangedEventArgs;
        }
    }

    public sealed class NewVersionDownloaded : Event
    {
    }

    public sealed class CheckForUpdateCompleted : Event
    {
    }

    public sealed class Updater
    {
        private const string ApplicationName = "DataCommander";
        private readonly Action<Event> _eventPublisher;
        private bool _updateStarted;
        public bool UpdateStarted => _updateStarted;
        public Updater(Action<Event> eventHandler) => _eventPublisher = eventHandler;

        public Task Update()
        {
            var command = DeploymentCommandRepository.Get(ApplicationName);
            return Handle((dynamic) command);
        }

        private async Task Handle(CheckForUpdates checkForUpdates)
        {
            if (checkForUpdates.When <= UniversalTime.Default.UtcNow)
            {
                var localVersion = Assembly.GetEntryAssembly().GetName().Version;
                var remoteVersionUri = new Uri("https://raw.githubusercontent.com/csbernath/DataCommander/master/DataCommander/Version.txt");
                _eventPublisher(new CheckForUpdatesStarted());
                var remoteVersion = await DeploymentApplication.GetRemoteVersion(remoteVersionUri);
                if (localVersion < remoteVersion)
                {
                    _eventPublisher(new DownloadingNewVersionStarted());
                    var address = new Uri($"https://github.com/csbernath/DataCommander/releases/download/{remoteVersion}/DataCommander.Updater.zip");
                    var guid = Guid.NewGuid();
                    var updaterDirectory = Path.Combine(Path.GetTempPath(), guid.ToString());
                    var zipFileName = Path.Combine(updaterDirectory, "Updater.zip");
                    await DeploymentApplication.DownloadUpdater(address, updaterDirectory, zipFileName,
                        args => _eventPublisher(new DownloadProgressChanged(args)));
                    _eventPublisher(new NewVersionDownloaded());
                    DeploymentApplication.ExtractZip(zipFileName, updaterDirectory);

                    var updaterExeFileName = Path.Combine(updaterDirectory, "DataCommander.Updater.exe");
                    DeploymentApplication.StartUpdater(updaterExeFileName);
                    _updateStarted = true;
                }
                else
                {
                    var now = UniversalTime.Default.UtcNow;
                    var when = now.Date.AddDays(1);
                    DeploymentCommandRepository.Save(ApplicationName, new CheckForUpdates {When = when});
                }
            }

            _eventPublisher(new CheckForUpdateCompleted());
        }

        private Task Handle(DeleteUpdater deleteUpdater)
        {
            DeploymentApplication.DeleteUpdater(deleteUpdater.Directory);
            return Task.CompletedTask;
        }
    }
}