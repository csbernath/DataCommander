using System;
using System.IO;
using System.Text;
using Foundation.Core;
using Foundation.Deployment.Commands;

namespace Foundation.Deployment;

public sealed class DeploymentCommandRepository
{
    private readonly ISerializer _serializer;

    public DeploymentCommandRepository(ISerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        _serializer = serializer;
    }

    public DeploymentCommand Get(string applicationName)
    {
        string fileName = GetFileName(applicationName);
        DeploymentCommand deploymentCommand;

        if (File.Exists(fileName))
        {
            string text = File.ReadAllText(fileName, Encoding.UTF8);
            deploymentCommand = _serializer.Deserialize<DeploymentCommand>(text);
        }
        else
        {
            DateTime now = UniversalTime.Default.Now;
            deploymentCommand = new CheckForUpdates(now);
        }

        return deploymentCommand;
    }

    public void Save(string applicationName, DeploymentCommand command)
    {
        string text = _serializer.Serialize(command);
        string fileName = GetFileName(applicationName);
        File.WriteAllText(fileName, text, Encoding.UTF8);
    }

    private static string GetFileName(string applicationName)
    {
        string localApplicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string directory = Path.Combine(localApplicationDataDirectory, applicationName);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        return Path.Combine(directory, "DeploymentCommand.json");
    }
}