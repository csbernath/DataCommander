using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Foundation.Deployment
{
    public static class DeploymentApplication
    {
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

        public static void ExtractZip(string zipFileName, string updaterDirectory)
        {
            ZipFile.ExtractToDirectory(zipFileName, updaterDirectory);
            File.Delete(zipFileName);
        }

        public static void StartUpdater(string updaterExeFileName)
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = updaterExeFileName;
            processStartInfo.Arguments = $"{Quote(Environment.CurrentDirectory)}";
            Process.Start(processStartInfo);
        }

        private static string Quote(string text)
        {
            return $"\"{text}\"";
        }

        public static void DeleteUpdater(string updaterDirectory)
        {
            Directory.Delete(updaterDirectory, true);
        }
    }
}