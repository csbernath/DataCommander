using System;
using System.Runtime.InteropServices;

namespace DataCommander.Application;

internal static class NativeMethods
{
    public static class Message
    {
        public enum EditBox
        {
            LineIndex = 0x00BB
        }

        public enum Gdi
        {
            SetRedraw = 0x000B
        }

        public enum Keyboard
        {
            KeyDown = 0x0100
        }

        public enum ScrollBar
        {
            VScroll = 0x0115
        }

        public enum ScrollBarParameter
        {
            ThumbPosition = 4
        }
    }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);
}