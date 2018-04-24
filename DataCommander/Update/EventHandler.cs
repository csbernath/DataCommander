using System.Windows.Forms;
using DataCommander.Update.Events;
using Foundation.Windows.Forms;

namespace DataCommander.Update
{
    internal sealed class EventHandler
    {
        private readonly UpdaterForm _updaterForm;
        public EventHandler(UpdaterForm updaterForm) => _updaterForm = updaterForm;
        public void Handle(Event @event) => Handle((dynamic) @event);
        private void Handle(CheckForUpdatesStarted @event) => _updaterForm.Invoke(() => _updaterForm.Log("Checking for updates..."));

        private void Handle(DownloadingNewVersionStarted @event) => _updaterForm.Invoke(() =>
        {
            _updaterForm.WindowState = FormWindowState.Normal;
            _updaterForm.Log($"Downloading new version {@event.Version}...");
        });

        private void Handle(DownloadProgressChanged @event) =>
            _updaterForm.Invoke(() => _updaterForm.Log($"{@event.DownloadProgressChangedEventArgs.ProgressPercentage}% complete."));

        private void Handle(NewVersionDownloaded @event) => _updaterForm.Invoke(() => _updaterForm.Log("New version downloaded."));

        private void Handle(ExceptionOccured @event) => _updaterForm.Invoke(() => _updaterForm.Log(@event.Exception.ToString()));
    }
}