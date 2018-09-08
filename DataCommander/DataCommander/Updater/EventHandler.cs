using System.Windows.Forms;
using Foundation.Deployment.Events;
using Foundation.Windows.Forms;

namespace DataCommander.Updater
{
    internal sealed class EventHandler
    {
        private readonly UpdaterForm _updaterForm;

        public EventHandler(UpdaterForm updaterForm) => _updaterForm = updaterForm;
        public void Handle(Event @event) => Handle((dynamic) @event);

        private void Handle(CheckForUpdatesStarted @event) => Log("Checking for updates...");

        private void Handle(DownloadingNewVersionStarted @event) => _updaterForm.Invoke(() =>
        {
            _updaterForm.WindowState = FormWindowState.Normal;
            _updaterForm.Log($"Downloading new version {@event.Version}...");
        });

        private void Handle(DownloadProgressChanged @event) => Log($"{@event.DownloadProgressChangedEventArgs.ProgressPercentage}% complete.");
        private void Handle(NewVersionDownloaded @event) => Log("New version downloaded.");
        private void Log(string message) => _updaterForm.Invoke(() => _updaterForm.Log(message));
    }
}