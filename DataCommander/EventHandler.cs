using System.Windows.Forms;
using Foundation.Windows.Forms;

namespace DataCommander
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
            Application.DoEvents();
            _updaterForm.Log("Downloading new version...");
            Application.DoEvents();
        });

        private void Handle(DownloadProgressChanged @event) => _updaterForm.Invoke(() => _updaterForm.Log($"{@event.DownloadProgressChangedEventArgs.ProgressPercentage}% complete."));
        private void Handle(NewVersionDownloaded @event) => _updaterForm.Invoke(() => _updaterForm.Log("New version downloaded."));

        private void Handle(CheckForUpdateCompleted @event) => _updaterForm.Invoke(() =>
        {
            _updaterForm.Log("Checking for updates completed.");
            Application.DoEvents();
            _updaterForm.Close();
            Application.DoEvents();
        });
    }
}