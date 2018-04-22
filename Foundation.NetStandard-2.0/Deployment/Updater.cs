using System.Diagnostics;
using System.IO;

namespace Foundation.Deployment
{
    public static class Updater
    {
        public static void Update(string updaterDirectory, string applicationExeFileName)
        {
            var applicationDirectory = Path.GetDirectoryName(applicationExeFileName);
            var backupDirectory = $"{applicationDirectory}.Backup";

            Directory.Move(applicationDirectory, backupDirectory);
            Directory.Move(updaterDirectory, applicationDirectory);
            Directory.Delete(backupDirectory);

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = applicationDirectory;
            processStartInfo.FileName = applicationExeFileName;
            processStartInfo.Arguments = applicationDirectory;
            Process.Start(processStartInfo);
        }
    }
}