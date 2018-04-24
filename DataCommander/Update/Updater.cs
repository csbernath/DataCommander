using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataCommander.Update.Events;
using Foundation;
using Foundation.Deployment;

namespace DataCommander.Update
{
    public sealed class Updater
    {
        private readonly Action<Event> _eventPublisher;
        private bool _updateStarted;
        public bool UpdateStarted => _updateStarted;
        public Updater(Action<Event> eventHandler) => _eventPublisher = eventHandler;

        public Task Update()
        {
            var entryAsembly = Assembly.GetEntryAssembly();
            var title = entryAsembly.GetCustomAttributes().OfType<AssemblyTitleAttribute>().First().Title;
            var applicationName = title;

            var command = DeploymentCommandRepository.Get(applicationName);
            return Handle((dynamic) command);
        }

        private async Task Handle(CheckForUpdates checkForUpdates)
        {
            if (checkForUpdates.When <= UniversalTime.Default.UtcNow)
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                var localVersion = entryAssembly.GetName().Version;
                var remoteVersionUri = new Uri("https://raw.githubusercontent.com/csbernath/DataCommander/master/Version.txt");
                _eventPublisher(new CheckForUpdatesStarted());
                var remoteVersion = await DeploymentHelper.GetRemoteVersion(remoteVersionUri);
                if (localVersion < remoteVersion)
                {
                    _eventPublisher(new DownloadingNewVersionStarted(remoteVersion));
                    var address = new Uri($"https://github.com/csbernath/DataCommander/releases/download/{remoteVersion}/DataCommander.Updater.zip");
                    var guid = Guid.NewGuid();
                    var updaterDirectory = Path.Combine(Path.GetTempPath(), guid.ToString());
                    var zipFileName = Path.Combine(updaterDirectory, "Updater.zip");
                    await DeploymentHelper.DownloadUpdater(address, updaterDirectory, zipFileName,
                        args => _eventPublisher(new DownloadProgressChanged(args)));
                    _eventPublisher(new NewVersionDownloaded());
                    DeploymentHelper.ExtractZip(zipFileName, updaterDirectory);

                    var updaterExeFileName = Path.Combine(updaterDirectory, "DataCommander.Updater.exe");
                    var applicationExeFileName = entryAssembly.Location;
                    DeploymentHelper.StartUpdater(updaterExeFileName, applicationExeFileName);
                    _updateStarted = true;
                }
                else
                    ScheduleCheckForUpdates();
            }
        }

        private static void ScheduleCheckForUpdates()
        {
            var entryAsembly = Assembly.GetEntryAssembly();
            var title = entryAsembly.GetCustomAttributes().OfType<AssemblyTitleAttribute>().First().Title;
            var applicationName = title;
            var now = UniversalTime.Default.UtcNow;
            var tomorrow = now.AddDays(1);
            DeploymentCommandRepository.Save(applicationName, new CheckForUpdates(tomorrow));
        }

        private Task Handle(DeleteUpdater deleteUpdater)
        {
            try
            {
                DeploymentHelper.DeleteUpdater(deleteUpdater.Directory);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            ScheduleCheckForUpdates();
            return Task.CompletedTask;
        }
    }
}