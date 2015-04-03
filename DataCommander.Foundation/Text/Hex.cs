namespace DataCommander.Foundation.Text
{
    using System;

    /// <summary>
    /// Hex encoding class.
    /// </summary>
    public static class Hex
    {
        private static readonly Char[] hexCharsUpper =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        private static readonly Char[] hexCharsLower =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
        };

        /// <summary>
        /// byte -> Char[2]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode(byte value, bool isUpper)
        {
            int d1 = (value & 0xF0) >> 4;
            int d0 = value & 0x0F;

            Char[] digits = new Char[2];
            Char[] hexChars = isUpper ? hexCharsUpper : hexCharsLower;

            digits[0] = hexChars[d1];
            digits[1] = hexChars[d0];

            return digits;
        }

        /// <summary>
        /// UInt16 --> Char[4]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static Char[] Encode(UInt16 value, bool isUpper)
        {
            int d3 = value >> 12;
            int d2 = (value & 0x0F00) >> 8;
            int d1 = (value & 0x00F0) >> 4;
            int d0 = value & 0x000F;

            Char[] digits = new Char[4];
            Char[] hexChars = isUpper ? hexCharsUpper : hexCharsLower;

            digits[0] = hexChars[d3];
            digits[1] = hexChars[d2];
            digits[2] = hexChars[d1];
            digits[3] = hexChars[d0];

            return digits;
        }

        /// <summary>
        /// int -> Char[8]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode(int value, bool isUpper)
        {
            int d7 = value >> 28;
            int d6 = (value & 0x0F000000) >> 24;
            int d5 = (value & 0x00F00000) >> 20;
            int d4 = (value & 0x000F0000) >> 16;
            int d3 = (value & 0x0000F000) >> 12;
            int d2 = (value & 0x00000F00) >> 8;
            int d1 = (value & 0x000000F0) >> 4;
            int d0 = value & 0x0000000F;

            Char[] digits = new Char[8];
            Char[] hexChars = isUpper ? hexCharsUpper : hexCharsLower;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode(byte[] bytes, bool isUpper)
        {
            int length = bytes.Length;
            return Encode(bytes, length, isUpper);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode(byte[] bytes, int length, bool isUpper)
        {
            Char[] chars = new Char[length << 1];
            int j = 0;
            Char[] hexChars = isUpper ? hexCharsUpper : hexCharsLower;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string GetString(UInt16 value, bool isUpper)
        {
            Char[] chars = Encode(value, isUpper);
            string s = new string(chars);
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static string GetString(int value, bool isUpper)
        {
            Char[] chars = Encode(value, isUpper);
            string s = new string(chars);
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static string GetString(UInt64 value, bool isUpper)
        {
            string format;

            if (isUpper)
            {
                format = "X";
            }
            else
            {
                format = "x";
            }

            string s = value.ToString(format).PadLeft(16, '0');
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static string GetString(byte[] bytes, bool isUpper)
        {
            Char[] chars = Encode(bytes, isUpper);
            string s = new string(chars);
            return s;
        }
    }
}