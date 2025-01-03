namespace Foundation.Diagnostics;

public sealed class WindowsCurrentVersion(string productName, string displayVersion, string editionId, string releaseId, string currentBuild)
{
    public readonly string ProductName = productName;
    public readonly string DisplayVersion = displayVersion;
    public readonly string EditionId = editionId;
    public readonly string ReleaseId = releaseId;
    public readonly string CurrentBuild = currentBuild;
}