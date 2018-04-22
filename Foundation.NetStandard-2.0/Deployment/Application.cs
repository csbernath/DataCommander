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

        public static async Task DownloadUpdater(Uri address, string updaterDirectory, string zipFileName)
        {
            Directory.CreateDirectory(updaterDirectory);

            using (var webClient = new WebClient())
                await webClient.DownloadFileTaskAsync(address, zipFileName);
        }

        public static void ExtractZip(string zipFileName, string updaterDirectory)
        {
            ZipFile.ExtractToDirectory(zipFileName, updaterDirectory);
            File.Delete(zipFileName);
        }

        public static void StartUpdater(string updaterExeFileName)
        {
            var workingDirectory = Path.GetDirectoryName(updaterExeFileName);

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = workingDirectory;
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