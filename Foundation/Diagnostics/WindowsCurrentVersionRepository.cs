using Microsoft.Win32;

namespace Foundation.Diagnostics;

public static class WindowsCurrentVersionRepository
{
    public static WindowsCurrentVersion Get()
    {
#pragma warning disable CA1416
        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")!;
        var productName = (string?)key.GetValue("ProductName");
        var displayVersion = (string?)key.GetValue("DisplayVersion");
        var editionId = (string?)key.GetValue("EditionID");
        var releaseId = (string?)key.GetValue("ReleaseId");
        var currentBuild = (string?)key.GetValue("CurrentBuild");
#pragma warning restore CA1416

        return new WindowsCurrentVersion(productName, displayVersion, editionId, releaseId, currentBuild);
    }
}