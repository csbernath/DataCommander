using Microsoft.Win32;

namespace Foundation.Diagnostics;

public static class WindowsCurrentVersionRepository
{
    public static WindowsCurrentVersion Get()
    {
#pragma warning disable CA1416
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            string productName = (string)key.GetValue("ProductName");
            string displayVersion = (string)key.GetValue("DisplayVersion");
            string editionId = (string)key.GetValue("EditionID");
            string releaseId = (string)key.GetValue("ReleaseId");
            string currentBuild = (string)key.GetValue("CurrentBuild");

            return new WindowsCurrentVersion(productName, displayVersion, editionId, releaseId, currentBuild);
        }
#pragma warning restore CA1416
    }
}