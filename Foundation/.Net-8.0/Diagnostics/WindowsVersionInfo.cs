using Microsoft.Win32;

namespace Foundation.Diagnostics;

public sealed class WindowsVersionInfo
{
    public readonly string ProductName;
    public readonly string DisplayVersion;
    public readonly string EditionId;
    public readonly string ReleaseId;
    public readonly string CurrentBuild;

    private WindowsVersionInfo(string productName, string displayVersion, string editionId, string releaseId, string currentBuild)
    {
        ProductName = productName;
        DisplayVersion = displayVersion;
        EditionId = editionId;
        ReleaseId = releaseId;
        CurrentBuild = currentBuild;
    }

    public static WindowsVersionInfo Get()
    {
#pragma warning disable CA1416
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            var productName = (string)key.GetValue("ProductName");
            var displayVersion = (string)key.GetValue("DisplayVersion");
            var editionId = (string)key.GetValue("EditionID");
            var releaseId = (string)key.GetValue("ReleaseId");
            var currentBuild = (string)key.GetValue("CurrentBuild");

            return new WindowsVersionInfo(productName, displayVersion, editionId, releaseId, currentBuild);
        }
#pragma warning restore CA1416
    }
}