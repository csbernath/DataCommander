using Microsoft.Win32;

namespace Foundation.Diagnostics
{
    public sealed class WindowsVersionInfo
    {
        public readonly string ProductName;
        public readonly string ReleaseId;
        public readonly string CurrentBuild;

        public WindowsVersionInfo(string productName, string releaseId, string currentBuild)
        {
            ProductName = productName;
            ReleaseId = releaseId;
            CurrentBuild = currentBuild;
        }

        public static WindowsVersionInfo Get()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                var productName = (string)key.GetValue("ProductName");
                var releaseId = (string)key.GetValue("ReleaseId");
                var currentBuild = (string)key.GetValue("CurrentBuild");
                return new WindowsVersionInfo(productName, releaseId, currentBuild);
            }
        }
    }
}