namespace DataCommander.Providers.Wmi
{
    using System;

    internal static class ManagementDateTimeConverter
    {
        public static DateTime ToDateTime( string dmtfDate )
        {
            string text2;
            var time2 = DateTime.MinValue;
            var num1 = time2.Year;
            time2 = DateTime.MinValue;
            var num2 = time2.Month;
            time2 = DateTime.MinValue;
            var num3 = time2.Day;
            time2 = DateTime.MinValue;
            var num4 = time2.Hour;
            time2 = DateTime.MinValue;
            var num5 = time2.Minute;
            time2 = DateTime.MinValue;
            var num6 = time2.Second;
            var num7 = 0;
            var text1 = dmtfDate;
            var time1 = DateTime.MinValue;
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
            var num8 = ((long) 0);
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
            var zone1 = TimeZone.CurrentTimeZone;
            var span1 = zone1.GetUtcOffset( time1 );
            var num9 = (span1.Ticks / ((long) 600000000));
            var num10 = 0;
            var text3 = text1.Substring( 22, 3 );
            var num11 = ((long) 0);

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