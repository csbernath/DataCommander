using System;

namespace Foundation.Text;

/// <summary>
/// Hex encoding class.
/// </summary>
public static class Hex
{
    private static readonly char[] HexCharsUpper = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'];
    private static readonly char[] HexCharsLower = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'];

    /// <summary>
    /// byte -> Char[2]
    /// </summary>
    public static char[] Encode(byte value, bool isUpper)
    {
        int d1 = (value & 0xF0) >> 4;
        int d0 = value & 0x0F;

        char[] digits = new char[2];
        char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;

        digits[0] = hexChars[d1];
        digits[1] = hexChars[d0];

        return digits;
    }

    /// <summary>
    /// UInt16 --> Char[4]
    /// </summary>
    [CLSCompliant(false)]
    public static char[] Encode(ushort value, bool isUpper)
    {
        int d3 = value >> 12;
        int d2 = (value & 0x0F00) >> 8;
        int d1 = (value & 0x00F0) >> 4;
        int d0 = value & 0x000F;

        char[] digits = new char[4];
        char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;

        digits[0] = hexChars[d3];
        digits[1] = hexChars[d2];
        digits[2] = hexChars[d1];
        digits[3] = hexChars[d0];

        return digits;
    }

    /// <summary>
    /// int -> Char[8]
    /// </summary>
    public static char[] Encode(int value, bool isUpper)
    {
        int d7 = value >> 28;
        int d6 = (value & 0x0F000000) >> 24;
        int d5 = (value & 0x00F00000) >> 20;
        int d4 = (value & 0x000F0000) >> 16;
        int d3 = (value & 0x0000F000) >> 12;
        int d2 = (value & 0x00000F00) >> 8;
        int d1 = (value & 0x000000F0) >> 4;
        int d0 = value & 0x0000000F;

        char[] digits = new char[8];
        char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;
        digits[0] = hexChars[d7];
        digits[1] = hexChars[d6];
        digits[2] = hexChars[d5];
        digits[3] = hexChars[d4];
        digits[4] = hexChars[d3];
        digits[5] = hexChars[d2];
        digits[6] = hexChars[d1];
        digits[7] = hexChars[d0];

        return digits;
    }

    public static char[] Encode(byte[] bytes, bool isUpper)
    {
        int length = bytes.Length;
        return Encode(bytes, length, isUpper);
    }

    public static char[] Encode(byte[] bytes, int length, bool isUpper)
    {
        char[] chars = new char[length << 1];
        int j = 0;
        char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;

        for (int i = 0; i < length; i++)
        {
            byte value = bytes[i];
            int d1 = (value & 0xF0) >> 4;
            int d0 = value & 0x0F;

            chars[j] = hexChars[d1];
            j++;
            chars[j] = hexChars[d0];
            j++;
        }

        return chars;
    }

    [CLSCompliant(false)]
    public static string GetString(ushort value, bool isUpper)
    {
        char[] chars = Encode(value, isUpper);
        string s = new string(chars);
        return s;
    }

    public static string GetString(int value, bool isUpper)
    {
        char[] chars = Encode(value, isUpper);
        string s = new string(chars);
        return s;
    }

    [CLSCompliant(false)]
    public static string GetString(ulong value, bool isUpper)
    {
        string format = isUpper ? "X" : "x";
        string s = value.ToString(format).PadLeft(16, '0');
        return s;
    }

    public static string GetString(byte[] bytes, bool isUpper)
    {
        char[] chars = Encode(bytes, isUpper);
        string s = new string(chars);
        return s;
    }
}