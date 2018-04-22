using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Foundation.Deployment
{
    public abstract class DeploymentCommand
    {
    }

    public class CheckForUpdates : DeploymentCommand
    {
        public readonly DateTime When;

        public CheckForUpdates(DateTime @when)
        {
            When = when;
        }
    }

    public class StartUpdater : DeploymentCommand
    {
    }

    public class DeleteUpdater : DeploymentCommand
    {
    }

    public static class DeploymentCommandRepository
    {
        public static DeploymentCommand Get(string applicationName)
        {
            var fileName = GetFileName(applicationName);
            DeploymentCommand deploymentCommand;

            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName, Encoding.UTF8);
                deploymentCommand = DataContractJsonSerialization.Deserialize<DeploymentCommand>(json);
            }
            else
                deploymentCommand = new CheckForUpdates(UniversalTime.Default.UtcNow);

            return deploymentCommand;
        }

        public static void Save(string applicationName, DeploymentCommand command)
        {
            var json = DataContractJsonSerialization.Serialize(command);
            var fileName = GetFileName(applicationName);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        private static string GetFileName(string applicationName)
        {
            var locallApplicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var directory = Path.Combine(locallApplicationDataDirectory, applicationName);
            return Path.Combine(directory, "DeploymentCommand.json");
        }
    }

    public static class DeploymentApplication
    {
        public static Version GetRemoteVersion(Uri address)
        {
            string text;
            using (var webClient = new WebClient())
                text = webClient.DownloadString(address);

            return new Version(text);
        }

        public static void DownloadUpdater(Uri address, string updaterDirectory)
        {
            Directory.CreateDirectory(updaterDirectory);
            var zipFileName = Path.Combine(updaterDirectory, "Updater.zip");

            using (var webClient = new WebClient())
                webClient.DownloadFile(address, zipFileName);

            ZipFile.ExtractToDirectory(zipFileName, updaterDirectory);
            File.Delete(zipFileName);
        }

        public static void StartUpdater(string updaterExeFileName)
        {
            var workingDirectory = Path.GetDirectoryName(updaterExeFileName);

            var processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = workingDirectory;
            processStartInfo.FileName = updaterExeFileName;
            processStartInfo.Arguments = Environment.CurrentDirectory;
            Process.Start(processStartInfo);
        }

        public static void DeleteUpdater(string updaterDirectory)
        {
            Directory.Delete(updaterDirectory, true);
        }
    }
}