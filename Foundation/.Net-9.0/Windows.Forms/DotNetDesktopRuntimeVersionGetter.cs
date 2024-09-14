using Foundation.Diagnostics;

namespace Foundation.Windows.Forms;

public class DotNetDesktopRuntimeVersionGetter
{
    public static string GetDotNetDesktopRuntimeVersion()
    {
        var dotNetRuntimeVersion = DotNetVersionGetter.GetVersion(typeof(System.Windows.Forms.Application));
        return dotNetRuntimeVersion;
    }
}