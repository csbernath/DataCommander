using System;
using System.IO;
using System.Reflection;
using Foundation;
using Foundation.Deployment;

namespace DataCommander
{
    internal static class Updater
    {
        private const string ApplicationName = "DataCommander";
        private static bool _updateStarted;

        public static bool UpdateStarted => _updateStarted;

        public static void Update()
        {
            var command = DeploymentCommandRepository.Get(ApplicationName);
            Handle((dynamic) command);
        }

        private static void Handle(CheckForUpdates checkForUpdates)
        {
            if (checkForUpdates.When <= UniversalTime.Default.UtcNow)
            {
                var localVersion = Assembly.GetEntryAssembly().GetName().Version;
                var remoteVersionUri = new Uri("https://raw.githubusercontent.com/csbernath/DataCommander/master/DataCommander/Version.txt");
                var remoteVersion = DeploymentApplication.GetRemoteVersion(remoteVersionUri);
                if (localVersion < remoteVersion)
                {
                    var address = new Uri($"https://github.com/csbernath/DataCommander/releases/download/{remoteVersion}/Updater.zip");
                    var guid = Guid.NewGuid();
                    var updaterDirectory = Path.Combine(Path.GetTempPath(), guid.ToString());
                    DeploymentApplication.DownloadUpdater(address, updaterDirectory);
                    string updaterExeFileName = Path.Combine(updaterDirectory, "DataCommander.Updater.exe");
                    DeploymentApplication.StartUpdater(updaterExeFileName);
                    _updateStarted = true;
                }
            }
        }
    }
}