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

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool MoveFileEx(
        string lpExistingFileName,
        string lpNewFileName,
        MoveFileExFlags flags);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);
}