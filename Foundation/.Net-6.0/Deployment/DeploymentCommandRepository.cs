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
        var fileName = GetFileName(applicationName);
        DeploymentCommand deploymentCommand;

        if (File.Exists(fileName))
        {
            var text = File.ReadAllText(fileName, Encoding.UTF8);
            deploymentCommand = _serializer.Deserialize<DeploymentCommand>(text);
        }
        else
        {
            var now = UniversalTime.Default.Now;
            deploymentCommand = new CheckForUpdates(now);
        }

        return deploymentCommand;
    }

    public void Save(string applicationName, DeploymentCommand command)
    {
        var text = _serializer.Serialize(command);
        var fileName = GetFileName(applicationName);
        File.WriteAllText(fileName, text, Encoding.UTF8);
    }

    private static string GetFileName(string applicationName)
    {
        var localApplicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var directory = Path.Combine(localApplicationDataDirectory, applicationName);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        return Path.Combine(directory, "DeploymentCommand.json");
    }
}