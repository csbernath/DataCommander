namespace DataCommander.Foundation.Text
{
    using System;

    /// <summary>
    /// Hex encoding class.
    /// </summary>
    public static class Hex
    {
        private static readonly Char[] HexCharsUpper = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private static readonly Char[] HexCharsLower = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        /// <summary>
        /// Byte -> Char[2]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode( Byte value, Boolean isUpper )
        {
            Int32 d1 = (value & 0xF0) >> 4;
            Int32 d0 = value & 0x0F;

            Char[] digits = new Char[ 2 ];
            Char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;

            digits[ 0 ] = hexChars[ d1 ];
            digits[ 1 ] = hexChars[ d0 ];

            return digits;
        }

        /// <summary>
        /// UInt16 --> Char[4]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        [CLSCompliant( false )]
        public static Char[] Encode( UInt16 value, Boolean isUpper )
        {
            Int32 d3 = value >> 12;
            Int32 d2 = (value & 0x0F00) >> 8;
            Int32 d1 = (value & 0x00F0) >> 4;
            Int32 d0 = value & 0x000F;

            Char[] digits = new Char[ 4 ];
            Char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;

            digits[ 0 ] = hexChars[ d3 ];
            digits[ 1 ] = hexChars[ d2 ];
            digits[ 2 ] = hexChars[ d1 ];
            digits[ 3 ] = hexChars[ d0 ];

            return digits;
        }

        /// <summary>
        /// Int32 -> Char[8]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode( Int32 value, Boolean isUpper )
        {
            Int32 d7 = value >> 28;
            Int32 d6 = (value & 0x0F000000) >> 24;
            Int32 d5 = (value & 0x00F00000) >> 20;
            Int32 d4 = (value & 0x000F0000) >> 16;
            Int32 d3 = (value & 0x0000F000) >> 12;
            Int32 d2 = (value & 0x00000F00) >> 8;
            Int32 d1 = (value & 0x000000F0) >> 4;
            Int32 d0 = value & 0x0000000F;

            Char[] digits = new Char[ 8 ];
            Char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;
            digits[ 0 ] = hexChars[ d7 ];
            digits[ 1 ] = hexChars[ d6 ];
            digits[ 2 ] = hexChars[ d5 ];
            digits[ 3 ] = hexChars[ d4 ];
            digits[ 4 ] = hexChars[ d3 ];
            digits[ 5 ] = hexChars[ d2 ];
            digits[ 6 ] = hexChars[ d1 ];
            digits[ 7 ] = hexChars[ d0 ];

            return digits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode( Byte[] bytes, Boolean isUpper )
        {
            Int32 length = bytes.Length;
            return Encode( bytes, length, isUpper );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static Char[] Encode( Byte[] bytes, Int32 length, Boolean isUpper )
        {
            Char[] chars = new Char[ length << 1 ];
            Int32 j = 0;
            Char[] hexChars = isUpper ? HexCharsUpper : HexCharsLower;

            for (Int32 i = 0; i < length; i++)
            {
                Byte value = bytes[ i ];
                Int32 d1 = (value & 0xF0) >> 4;
                Int32 d0 = value & 0x0F;

                chars[ j ] = hexChars[ d1 ];
                j++;
                chars[ j ] = hexChars[ d0 ];
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
        [CLSCompliant( false )]
        public static String GetString( UInt16 value, Boolean isUpper )
        {
            Char[] chars = Encode( value, isUpper );
            String s = new String( chars );
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static String GetString( Int32 value, Boolean isUpper )
        {
            Char[] chars = Encode( value, isUpper );
            String s = new String( chars );
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        [CLSCompliant( false )]
        public static String GetString( UInt64 value, Boolean isUpper )
        {
            String format;

            if (isUpper)
            {
                format = "X";
            }
            else
            {
                format = "x";
            }

            String s = value.ToString( format ).PadLeft( 16, '0' );
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isUpper"></param>
        /// <returns></returns>
        public static String GetString( Byte[] bytes, Boolean isUpper )
        {
            Char[] chars = Encode( bytes, isUpper );
            String s = new String( chars );
            return s;
        }
    }
}