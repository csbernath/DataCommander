namespace Foundation.Diagnostics;

public static class WindowsNameCalculator
{
    public static string GetWindowsNameFromBuildNumber(int buildNumber, string editionId, string displayVersion)
    {
        string productName = GetProductName(buildNumber);
        string windowsName = $"Windows {productName} {editionId} version {displayVersion}";
        return windowsName;
    }

    private static string GetProductName(int buildNumber)
    {
        string productName = buildNumber switch
        {
            < 9200 => "7",
            < 10240 => "8",
            < 22000 => "10",
            _ => "11"
        };
        return productName;
    }
}