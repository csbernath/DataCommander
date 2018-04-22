using System.Diagnostics;
using System.IO;

namespace Foundation.Deployment
{
    public static class Updater
    {
        private const string ApplicationName = "DataCommander";

        public static void Update(string updaterDirectory, string applicationExeFileName)
        {
            var applicationDirectory = Path.GetDirectoryName(applicationExeFileName);
            var backupDirectory = $"{applicationDirectory}.Backup";

            Directory.Move(applicationDirectory, backupDirectory);
            Directory.Move(updaterDirectory, applicationDirectory);
            Directory.Delete(backupDirectory);

            DeploymentCommandRepository.Save(ApplicationName, new DeleteUpdater());

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = applicationDirectory;
            processStartInfo.FileName = applicationExeFileName;
            processStartInfo.Arguments = applicationDirectory;
            Process.Start(processStartInfo);
        }
    }
}