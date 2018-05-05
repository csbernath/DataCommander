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
    }
}