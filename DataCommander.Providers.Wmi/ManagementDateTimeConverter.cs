namespace DataCommander.Providers.Wmi
{
    using System;

    internal static class ManagementDateTimeConverter
    {
        public static DateTime ToDateTime( string dmtfDate )
        {
            string text2;
            DateTime time2 = DateTime.MinValue;
            int num1 = time2.Year;
            time2 = DateTime.MinValue;
            int num2 = time2.Month;
            time2 = DateTime.MinValue;
            int num3 = time2.Day;
            time2 = DateTime.MinValue;
            int num4 = time2.Hour;
            time2 = DateTime.MinValue;
            int num5 = time2.Minute;
            time2 = DateTime.MinValue;
            int num6 = time2.Second;
            int num7 = 0;
            string text1 = dmtfDate;
            DateTime time1 = DateTime.MinValue;
            if (text1 == null)
            {
                throw new ArgumentOutOfRangeException();

            }
            if (text1.Length == 0)
            {
                throw new ArgumentOutOfRangeException();

            }
            if (text1.Length != 25)
            {
                throw new ArgumentOutOfRangeException();

            }
            long num8 = ((long) 0);
            try
            {
                text2 = string.Empty;
                text2 = text1.Substring( 0, 4 );
                if ("****" != text2)
                {
                    num1 = int.Parse( text2 );

                }
                text2 = text1.Substring( 4, 2 );
                if ("**" != text2)
                {
                    num2 = int.Parse( text2 );

                }
                text2 = text1.Substring( 6, 2 );
                if ("**" != text2)
                {
                    num3 = int.Parse( text2 );

                }
                text2 = text1.Substring( 8, 2 );
                if ("**" != text2)
                {
                    num4 = int.Parse( text2 );

                }
                text2 = text1.Substring( 10, 2 );
                if ("**" != text2)
                {
                    num5 = int.Parse( text2 );

                }
                text2 = text1.Substring( 12, 2 );
                if ("**" != text2)
                {
                    num6 = int.Parse( text2 );

                }
                text2 = text1.Substring( 15, 6 );
                if ("******" != text2)
                {
                    num8 = (long.Parse( text2 ) * ((long) 10));

                }
                if ((((num1 >= 0) && (num2 >= 0)) && ((num3 >= 0) && (num4 >= 0))) && (((num5 >= 0) && (num6 >= 0)) && (num8 >= ((long) 0))))
                {
                    goto Label_01BE;

                }
                throw new ArgumentOutOfRangeException();

            }
            catch
            {
                throw new ArgumentOutOfRangeException();

            }

Label_01BE:
            time1 = new DateTime( num1, num2, num3, num4, num5, num6, num7 );
            time1 = time1.AddTicks( num8 );
            TimeZone zone1 = TimeZone.CurrentTimeZone;
            TimeSpan span1 = zone1.GetUtcOffset( time1 );
            long num9 = (span1.Ticks / ((long) 600000000));
            int num10 = 0;
            string text3 = text1.Substring( 22, 3 );
            long num11 = ((long) 0);

            if ("***" == text3)
            {
                return time1;
            }
            text3 = text1.Substring( 21, 4 );
            try
            {
                num10 = int.Parse( text3 );

            }
            catch
            {
                throw new ArgumentOutOfRangeException();

            }
            num11 = (((long) num10) - num9);
            return time1.AddMinutes( ((double) (num11 * ((long) -1))) );
        }
    }
}