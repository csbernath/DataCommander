using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Foundation.Deployment
{
    public static class DeploymentHelper
    {
        private const string ApplicationName = "DataCommander";

        public static async Task<Version> GetRemoteVersion(Uri address)
        {
            string text;
            using (var webClient = new WebClient())
                text = await webClient.DownloadStringTaskAsync(address);

            return new Version(text);
        }

        public static async Task DownloadUpdater(Uri address, string updaterDirectory, string zipFileName,
            Action<DownloadProgressChangedEventArgs> eventHandler)
        {
            Directory.CreateDirectory(updaterDirectory);

            var sequence = new Sequence();
            var previousEventTimestamp = 0;

            using (var webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += (sender, args) =>
                {
                    if (sequence.Next() == 0)
                    {
                        previousEventTimestamp = UniversalTime.GetTickCount();
                        eventHandler(args);
                    }
                    else
                    {
                        var current = UniversalTime.GetTickCount();
                        var elapsed = current - previousEventTimestamp;
                        if (elapsed >= 1000)
                        {
                            previousEventTimestamp = current;
                            eventHandler(args);
                        }
                    }
                };
                await webClient.DownloadFileTaskAsync(address, zipFileName);
            }
        }

        public static void ExtractZip(string sourceArchiveFileName, string destinationDirectoryName)
        {
            ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
            File.Delete(sourceArchiveFileName);
        }

        public static void StartUpdater(string updaterExeFileName, string applicationExeFileName)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = updaterExeFileName;
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(updaterExeFileName);
            processStartInfo.Arguments = $"{Quote(applicationExeFileName)}";
            Process.Start(processStartInfo);
        }

        public static void Update(string updaterDirectory, string applicationExeFileName)
        {
            var applicationDirectory = Path.GetDirectoryName(applicationExeFileName);
            var backupDirectory = $"{applicationDirectory}.Backup";

            Directory.Move(applicationDirectory, backupDirectory);
            Directory.Move(updaterDirectory, applicationDirectory);
            Directory.Delete(backupDirectory);

            DeploymentCommandRepository.Save(ApplicationName, new DeleteUpdater { Directory = updaterDirectory });

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = applicationDirectory;
            processStartInfo.FileName = applicationExeFileName;
            processStartInfo.Arguments = applicationDirectory;
            Process.Start(processStartInfo);
        }

        public static void DeleteUpdater(string updaterDirectory)
        {
            Directory.Delete(updaterDirectory, true);
        }

        private static string Quote(string text)
        {
            return $"\"{text}\"";
        }
    }
}