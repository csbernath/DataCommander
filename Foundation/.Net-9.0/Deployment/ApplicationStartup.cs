using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Foundation.Core;
using Foundation.Deployment.Commands;
using Foundation.Deployment.Events;

namespace Foundation.Deployment;

public sealed class ApplicationStartup(
    ISerializer serializer,
    Uri remoteVersionUri,
    string address,
    Action<Event> eventHandler)
{
    private bool _updateStarted;

    public bool UpdateStarted => _updateStarted;

    public Task Update()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var title = GetTitle(entryAssembly);
        var applicationName = title;

        var repository = new DeploymentCommandRepository(serializer);

        DeploymentCommand command;
        try
        {
            command = repository.Get(applicationName);
        }
        catch
        {
            var now = UniversalTime.Default.Now;
            command = new CheckForUpdates(now);
        }

        return Handle((dynamic)command);
    }

    private static void DeleteUpdater(string updaterDirectory) => Directory.Delete(updaterDirectory, true);

    private static async Task DownloadUpdater(Uri address, string updaterDirectory, string zipFileName,
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
                    previousEventTimestamp = Environment.TickCount;
                    eventHandler(args);
                }
                else
                {
                    var current = Environment.TickCount;
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

    private static void ExtractZip(string sourceArchiveFileName, string destinationDirectoryName)
    {
        ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
        File.Delete(sourceArchiveFileName);
    }

    private static async Task<Version> GetRemoteVersion(Uri address)
    {
        string text;
        using (var webClient = new WebClient())
            text = await webClient.DownloadStringTaskAsync(address);

        return new Version(text);
    }

    private static string GetTitle(Assembly entryAsembly)
    {
        var title = entryAsembly.GetCustomAttributes().OfType<AssemblyTitleAttribute>().First().Title;
        return title;
    }

    private async Task Handle(CheckForUpdates checkForUpdates)
    {
        if (checkForUpdates.When <= UniversalTime.Default.Now)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var localVersion = entryAssembly.GetName().Version;
            eventHandler(new CheckForUpdatesStarted());
            var remoteVersion = await GetRemoteVersion(remoteVersionUri);
            if (localVersion < remoteVersion)
            {
                eventHandler(new DownloadingNewVersionStarted(remoteVersion));
                var address1 = new Uri(string.Format(address, (object)remoteVersion));
                var guid = Guid.NewGuid();
                var updaterDirectory = Path.Combine(Path.GetTempPath(), guid.ToString());
                var zipFileName = Path.Combine(updaterDirectory, "Updater.zip");
                await DownloadUpdater(address1, updaterDirectory, zipFileName,
                    args => eventHandler(new DownloadProgressChanged(args)));
                eventHandler(new NewVersionDownloaded());
                ExtractZip(zipFileName, updaterDirectory);

                var updaterExeFileName = Path.Combine(updaterDirectory, "DataCommander.Updater.exe");
                var applicationExeFileName = entryAssembly.Location;
                StartUpdater(updaterExeFileName, applicationExeFileName);
                _updateStarted = true;
            }
            else
                ScheduleCheckForUpdates();
        }
    }

    private Task Handle(DeleteUpdater deleteUpdater)
    {
        DeleteUpdater(deleteUpdater.Directory);
        ScheduleCheckForUpdates();
        return Task.CompletedTask;
    }

    private static string Quote(string text) => $"\"{text}\"";

    private void ScheduleCheckForUpdates()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var title = GetTitle(entryAssembly);
        var applicationName = title;
        var now = UniversalTime.Default.Now;
        var tomorrow = now.AddDays(1);

        var repository = new DeploymentCommandRepository(serializer);
        repository.Save(applicationName, new CheckForUpdates(tomorrow));
    }

    private static void StartUpdater(string updaterExeFileName, string applicationExeFileName)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = updaterExeFileName,
            WorkingDirectory = Path.GetDirectoryName(updaterExeFileName),
            Arguments = $"{Quote(applicationExeFileName)}"
        };
        Process.Start(processStartInfo);
    }
}