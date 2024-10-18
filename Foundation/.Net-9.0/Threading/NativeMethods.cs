using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Foundation.Threading;

/// <exclude/>
/// <summary>
/// Wrapper class for some NativeMethods. API functions (KERNEL32.DLL, USER32.DLL)
/// and constants declared in WinUser.h.
/// </summary>
public static class NativeMethods
{
    [Flags]
    public enum MoveFileExFlags
    {
        ReplaceExisiting = 0x00000001,
        CopyAllowed = 0x00000002,
        DelayUntilReboot = 0x00000004,
        WriteThrough = 0x00000008,
        CreateHardLink = 0x00000010,
        FailIfNotTrackable = 0x00000020
    }

    [DllImport("kernel32")]
    private static extern short FreeLibrary(int hModule);

    [DllImport("kernel32")]
    private static extern uint GetCurrentThreadId();

    //[DllImport("kernel32", CharSet = CharSet.Auto)]
    //private static extern uint GetShortPathName(
    //    string lpszLongPath,
    //    StringBuilder lpszShortPath,
    //    uint cchBuffer);

    //[DllImport("kernel32")]
    //private static extern int LoadLibrary(string fileName);

    [DllImport("user32")]
    private static extern int LoadString(int hInstance, int id, byte[] buf, int buflen);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool MoveFileEx(
        string lpExistingFileName,
        string lpNewFileName,
        MoveFileExFlags flags);
}