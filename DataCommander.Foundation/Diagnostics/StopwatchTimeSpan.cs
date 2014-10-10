namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public struct StopwatchTimeSpan
    {
        #region Public Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly Double TicksPerTick = (Double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Double TicksPerMicrosecond = Stopwatch.Frequency / 1000000.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Double TicksPerMillisecond = Stopwatch.Frequency / 1000.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int64 TicksPerSecond = Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int64 TicksPerMinute = 60 * TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int64 TicksPerHour = 60 * TicksPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int64 TicksPerDay = 24 * TicksPerHour;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int32 SecondsPerMinute = 60;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int32 SecondsPerHour = 60 * SecondsPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int32 SecondsPerDay = 24 * SecondsPerHour;

        #endregion

        #region Private Fields

        private Int64 ticks;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        public StopwatchTimeSpan( Int64 ticks )
        {
            this.ticks = ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        public StopwatchTimeSpan( TimeSpan timeSpan )
        {
            this.ticks = ToTicks( timeSpan );
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Ticks
        {
            get
            {
                return this.ticks;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                return ToTimeSpan( this.ticks );                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double TotalHours
        {
            get
            {
                return (Double) this.ticks / TicksPerHour;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double TotalMinutes
        {
            get
            {
                return (Double) this.ticks / TicksPerMinute;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double TotalSeconds
        {
            get
            {
                return (Double) this.ticks / TicksPerSecond;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double TotalMilliseconds
        {
            get
            {
                return (Double) this.ticks * 1000 / TicksPerSecond;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double TotalMicroseconds
        {
            get
            {
                return (Double) this.ticks * 1000000 / TicksPerSecond;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double TotalNanoseconds
        {
            get
            {
                return (Double) this.ticks * 1000000000 / TicksPerSecond;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Int32 ToInt32( Int64 ticks, Int32 multiplier )
        {
            Double d = (Double) multiplier * ticks / TicksPerSecond;
            Int32 int32 = (Int32) Math.Round( d );
            return int32;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Int64 ToInt64( Int64 ticks, Int64 multiplier )
        {
            Double d = (Double) multiplier * ticks / Stopwatch.Frequency;
            Int64 int64 = (Int64) Math.Round( d );
            return int64;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static String ToString( Int64 ticks, Int32 scale )
        {
            Int64 totalSeconds = ticks / TicksPerSecond;
            String fractionString = null;

            if (scale > 0)
            {
                String multiplierString = "1" + new String( '0', scale );
                Int64 multiplier = Int64.Parse( multiplierString );
                Int64 fractionTicks = ticks - (totalSeconds * TicksPerSecond);
                Double fraction = (Double) multiplier * fractionTicks / TicksPerSecond;
                fraction = Math.Round( fraction );
                Int64 fractionInt64 = (Int64) fraction;
                if (fractionInt64 < multiplier)
                {
                    fractionString = String.Format( ".{0}", fractionInt64.ToString().PadLeft( scale, '0' ) );
                }
                else
                {
                    totalSeconds++;
                }
            }

            var sb = new StringBuilder();
            Int64 days = ticks / TicksPerDay;

            if (days != 0)
            {
                totalSeconds -= SecondsPerDay * days;

                sb.Append( days );
                sb.Append( '.' );
            }

            Int64 hours = totalSeconds / 3600;
            Int32 seconds = (Int32) (totalSeconds - (hours * 3600));
            Int32 minutes = seconds / 60;
            seconds -= minutes * 60;

            if (sb.Length > 0 || hours > 0)
            {
                sb.Append( hours.ToString().PadLeft( 2, '0' ) );
                sb.Append( ':' );
            }

            sb.Append( minutes.ToString().PadLeft( 2, '0' ) );
            sb.Append( ':' );
            sb.Append( seconds.ToString().PadLeft( 2, '0' ) );

            String s = String.Format( "{0}{1}", sb, fractionString );
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static Int64 ToTicks( TimeSpan timeSpan )
        {
            return (Int64) (timeSpan.TotalSeconds * Stopwatch.Frequency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan( Int64 elapsed )
        {
            Int64 ticks = (Int64) ( elapsed*TicksPerTick );
            TimeSpan timeSpan = new TimeSpan( ticks );
            return timeSpan;
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public String ToString( Int32 scale )
        {
            return ToString( this.ticks, scale );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return ToString( this.ticks, 9 );
        }
    }
}