using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Foundation.Setup
{
    public static class Installer
    {
        public static string DownloadString(string address)
        {
            using (var webClient = new WebClient())
                return webClient.DownloadString(address);
        }

        public static async Task Download(Uri address, string setupExeFileName)
        {
            var guid = Guid.NewGuid();
            var directory = Path.Combine(Path.GetTempPath(), $"Foundation.Setup.{guid}");
            var zipFilenName = $"{guid}.zip";

            using (var webClient = new WebClient())
                await webClient.DownloadFileTaskAsync(address, zipFilenName);

            var sourceArchiveFileName = Path.Combine(directory, zipFilenName);
            ZipFile.ExtractToDirectory(sourceArchiveFileName, directory);

            File.Delete(sourceArchiveFileName);

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = directory;
            processStartInfo.FileName = setupExeFileName;
            processStartInfo.Arguments = Environment.CurrentDirectory;
            Process.Start(processStartInfo);
        }

        public static void Start(string sourceDirectory, string targetDirectory, string exeFileName)
        {
            var backupDirectory = $"{targetDirectory}.Backup";
            Directory.Move(targetDirectory, backupDirectory);
            Directory.Move(sourceDirectory, targetDirectory);
            Directory.Delete(backupDirectory);

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = targetDirectory;
            processStartInfo.FileName = exeFileName;
            processStartInfo.Arguments = Environment.CurrentDirectory;
            Process.Start(processStartInfo);
        }
    }
}