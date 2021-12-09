using Microsoft.Win32;

namespace Foundation.Diagnostics;

public sealed class WindowsVersionInfo
{
    public readonly string ProductName;
    public readonly string DisplayVersion;
    public readonly string ReleaseId;
    public readonly string CurrentBuild;

    public WindowsVersionInfo(string productName, string displayVersion, string releaseId, string currentBuild)
    {
        ProductName = productName;
        DisplayVersion = displayVersion;
        ReleaseId = releaseId;
        CurrentBuild = currentBuild;
    }

    public static WindowsVersionInfo Get()
    {
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            var productName = (string)key.GetValue("ProductName");
            var displayVersion = (string) key.GetValue("DisplayVersion");
            var releaseId = (string)key.GetValue("ReleaseId");
            var currentBuild = (string)key.GetValue("CurrentBuild");
            return new WindowsVersionInfo(productName, displayVersion, releaseId, currentBuild);
        }
    }
}