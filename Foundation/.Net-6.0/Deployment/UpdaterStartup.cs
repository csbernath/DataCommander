using System;
using System.Diagnostics;
using System.IO;
using Foundation.Assertions;
using Foundation.Core;
using Foundation.Deployment.Commands;

namespace Foundation.Deployment;

public sealed class UpdaterStartup
{
    private readonly ISerializer _serializer;

    public UpdaterStartup(ISerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        _serializer = serializer;
    }

    public void Update(string applicationName, string updaterDirectory, string applicationExeFileName)
    {
        var applicationDirectory = Path.GetDirectoryName(applicationExeFileName);

        var backupDirectory = Path.Combine(updaterDirectory, $"{applicationName}.Backup");
        CopyDirectory(applicationDirectory, backupDirectory);

        var sourceDirectory = Path.Combine(updaterDirectory, applicationName);
        CopyDirectory(sourceDirectory, applicationDirectory);

        var repository = new DeploymentCommandRepository(_serializer);
        repository.Save(applicationName, new DeleteUpdater(updaterDirectory));

        var processStartInfo = new ProcessStartInfo
        {
            WorkingDirectory = applicationDirectory,
            FileName = applicationExeFileName,
            Arguments = applicationDirectory
        };
        Process.Start(processStartInfo);
    }

    private static void CopyDirectory(string sourceDirectoryName, string targetDirectoryName)
    {
        var sourceDirectory = new DirectoryInfo(sourceDirectoryName);
        var targetDirectory = new DirectoryInfo(targetDirectoryName);
        Copy(sourceDirectory, targetDirectory);
    }

    private static void Copy(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
    {
        if (!targetDirectory.Exists)
            targetDirectory.Create();

        foreach (var file in sourceDirectory.GetFiles())
            Copy(file, targetDirectory);

        foreach (var sourceSubDirectory in sourceDirectory.GetDirectories())
        {
            var targetSubDirectoryName = Path.Combine(targetDirectory.FullName, sourceSubDirectory.Name);
            var targetSubDirectory = new DirectoryInfo(targetSubDirectoryName);
            Copy(sourceSubDirectory, targetSubDirectory);
        }
    }

    private static void Copy(FileInfo sourceFile, DirectoryInfo targetDirectory)
    {
        var targetFileName = Path.Combine(targetDirectory.FullName, sourceFile.Name);
        sourceFile.CopyTo(targetFileName);
    }
}