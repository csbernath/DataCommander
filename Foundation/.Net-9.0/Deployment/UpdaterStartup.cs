using System;
using System.Diagnostics;
using System.IO;
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
        string applicationDirectory = Path.GetDirectoryName(applicationExeFileName);

        string backupDirectory = Path.Combine(updaterDirectory, $"{applicationName}.Backup");
        CopyDirectory(applicationDirectory, backupDirectory);

        string sourceDirectory = Path.Combine(updaterDirectory, applicationName);
        CopyDirectory(sourceDirectory, applicationDirectory);

        DeploymentCommandRepository repository = new DeploymentCommandRepository(_serializer);
        repository.Save(applicationName, new DeleteUpdater(updaterDirectory));

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            WorkingDirectory = applicationDirectory,
            FileName = applicationExeFileName,
            Arguments = applicationDirectory
        };
        Process.Start(processStartInfo);
    }

    private static void CopyDirectory(string sourceDirectoryName, string targetDirectoryName)
    {
        DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryName);
        DirectoryInfo targetDirectory = new DirectoryInfo(targetDirectoryName);
        Copy(sourceDirectory, targetDirectory);
    }

    private static void Copy(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
    {
        if (!targetDirectory.Exists)
            targetDirectory.Create();

        foreach (FileInfo file in sourceDirectory.GetFiles())
            Copy(file, targetDirectory);

        foreach (DirectoryInfo sourceSubDirectory in sourceDirectory.GetDirectories())
        {
            string targetSubDirectoryName = Path.Combine(targetDirectory.FullName, sourceSubDirectory.Name);
            DirectoryInfo targetSubDirectory = new DirectoryInfo(targetSubDirectoryName);
            Copy(sourceSubDirectory, targetSubDirectory);
        }
    }

    private static void Copy(FileInfo sourceFile, DirectoryInfo targetDirectory)
    {
        string targetFileName = Path.Combine(targetDirectory.FullName, sourceFile.Name);
        sourceFile.CopyTo(targetFileName);
    }
}