using System;
using System.IO;
using System.Text;

namespace Foundation.Deployment
{
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
            {
                var now = UniversalTime.Default.UtcNow;
                deploymentCommand = new CheckForUpdates(now);
            }

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
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return Path.Combine(directory, "DeploymentCommand.json");
        }
    }
}