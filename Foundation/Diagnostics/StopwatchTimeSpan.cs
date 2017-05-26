using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public struct StopwatchTimeSpan
    {
        #region Public Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly double TicksPerTick = (double)TimeSpan.TicksPerSecond/Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double TicksPerMicrosecond = Stopwatch.Frequency/1000000.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double TicksPerMillisecond = Stopwatch.Frequency/1000.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerSecond = Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerMinute = 60*TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerHour = 60*TicksPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerDay = 24*TicksPerHour;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int SecondsPerMinute = 60;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int SecondsPerHour = 60*SecondsPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int SecondsPerDay = 24*SecondsPerHour;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        public StopwatchTimeSpan(long ticks)
        {
            this.Ticks = ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        public StopwatchTimeSpan(TimeSpan timeSpan)
        {
            this.Ticks = ToTicks(timeSpan);
        }

        /// <summary>
        /// 
        /// </summary>
        public long Ticks { get; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Elapsed => ToTimeSpan(this.Ticks);

        /// <summary>
        /// 
        /// </summary>
        public double TotalHours => (double)this.Ticks/TicksPerHour;

        /// <summary>
        /// 
        /// </summary>
        public double TotalMinutes => (double)this.Ticks/TicksPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public double TotalSeconds => (double)this.Ticks/TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public double TotalMilliseconds => (double)this.Ticks*1000/TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public double TotalMicroseconds => (double)this.Ticks*1000000/TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public double TotalNanoseconds => (double)this.Ticks*1000000000/TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static int ToInt32(long ticks, int multiplier)
        {
            var d = (double)multiplier*ticks/TicksPerSecond;
            var int32 = (int)Math.Round(d);
            return int32;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static long ToInt64(long ticks, long multiplier)
        {
            var d = (double)multiplier*ticks/Stopwatch.Frequency;
            var int64 = (long)Math.Round(d);
            return int64;
        }

        private static readonly long[] power10 = new [] 
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000,
            10000000000,
            100000000000,
            1000000000000,
            10000000000000,
            100000000000000,
            1000000000000000
        };

        private static long Pow10(int pow)
        {
            return power10[pow];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static string ToString(long ticks, int scale)
        {
            var totalSeconds = ticks/TicksPerSecond;
            string fractionString = null;

            if (scale > 0)
            {
                var fractionTicks = ticks - (totalSeconds*TicksPerSecond);
                var multiplier = Pow10(scale);
                var fraction = (double)multiplier*fractionTicks/TicksPerSecond;
                fraction = Math.Round(fraction);
                var fractionInt64 = (long)fraction;
                if (fractionInt64 == multiplier)
                {
                    fractionInt64 = 0;
                    totalSeconds++;

                }
                fractionString = $".{fractionInt64.ToString().PadLeft(scale, '0')}";
            }

            var sb = new StringBuilder();
            var days = ticks/TicksPerDay;

            if (days != 0)
            {
                totalSeconds -= SecondsPerDay*days;

                sb.Append(days);
                sb.Append('.');
            }

            var hours = totalSeconds/3600;
            var seconds = (int)(totalSeconds - (hours*3600));
            var minutes = seconds/60;
            seconds -= minutes*60;

            if (sb.Length > 0 || hours > 0)
            {
                sb.Append(hours.ToString().PadLeft(2, '0'));
                sb.Append(':');
            }

            sb.Append(minutes.ToString().PadLeft(2, '0'));
            sb.Append(':');
            sb.Append(seconds.ToString().PadLeft(2, '0'));

            var s = $"{sb}{fractionString}";
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static long ToTicks(TimeSpan timeSpan)
        {
            return (long)(timeSpan.TotalSeconds*Stopwatch.Frequency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(long elapsed)
        {
            var ticks = (long)(elapsed*TicksPerTick);
            return new TimeSpan(ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public string ToString(int scale)
        {
            return ToString(this.Ticks, scale);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(this.Ticks, 9);
        }
    }
}