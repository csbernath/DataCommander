using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Foundation.IO;

/// <exclude/>
/// <summary>
/// Wrapper class for some NativeMethods. API functions (KERNEL32.DLL, USER32.DLL)
/// and constants declared in WinUser.h.
/// </summary>
public static class NativeMethods
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum MoveFileExFlags
    {
        /// <summary>
        /// 
        /// </summary>
        ReplaceExisiting = 0x00000001,

        /// <summary>
        /// 
        /// </summary>
        CopyAllowed = 0x00000002,

        /// <summary>
        /// 
        /// </summary>
        DelayUntilReboot = 0x00000004,

        /// <summary>
        /// 
        /// </summary>
        WriteThrough = 0x00000008,

        /// <summary>
        /// 
        /// </summary>
        CreateHardLink = 0x00000010,

        /// <summary>
        /// 
        /// </summary>
        FailIfNotTrackable = 0x00000020
    }

    [DllImport("kernel32")]
    public static extern short FreeLibrary(int hModule);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    public static extern uint GetShortPathName(
        string lpszLongPath,
        StringBuilder lpszShortPath,
        uint cchBuffer);

    [DllImport("kernel32")]
    public static extern int LoadLibrary(string fileName);

    [DllImport("user32")]
    public static extern int LoadString(int hInstance, int id, byte[] buf, int buflen);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lpExistingFileName"></param>
    /// <param name="lpNewFileName"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool MoveFileEx(
        string lpExistingFileName,
        string lpNewFileName,
        MoveFileExFlags flags);
}