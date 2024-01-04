namespace Foundation.Diagnostics;

public static class WindowsNameCalculator
{
    public static string GetWindowsNameFromBuildNumber(int buildNumber, string editionId, string displayVersion)
    {
        var productName = GetProductName(buildNumber);
        var windowsName = $"Windows {productName} {editionId} version {displayVersion}";
        return windowsName;
    }

    private static string GetProductName(int buildNumber)
    {
        string productName;
        if (buildNumber < 9200)
            productName = "7";
        else if (buildNumber < 10240)
            productName = "8";
        else if (buildNumber < 22000)
            productName = "10";
        else
            productName = "11";
        return productName;
    }
}