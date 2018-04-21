using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Foundation.Setup
{
    public static class Installer
    {
        public static string DownloadString(string address)
        {
            using (var webClient = new WebClient())
                return webClient.DownloadString(address);
        }

        public static void Download(Uri address, string setupExeFileName)
        {
            var guid = Guid.NewGuid();
            var directory = Path.Combine(Path.GetTempPath(), $"Foundation.Setup.{guid}");
            Directory.CreateDirectory(directory);
            var zipFileName = Path.Combine(directory, $"{guid}.zip");

            using (var webClient = new WebClient())
                webClient.DownloadFile(address, zipFileName);

            ZipFile.ExtractToDirectory(zipFileName, directory);

            File.Delete(zipFileName);

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