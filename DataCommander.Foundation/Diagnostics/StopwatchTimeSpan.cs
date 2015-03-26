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
        public static readonly long TicksPerSecond = Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerMinute = 60 * TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerHour = 60 * TicksPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerDay = 24 * TicksPerHour;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int SecondsPerMinute = 60;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int SecondsPerHour = 60 * SecondsPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int SecondsPerDay = 24 * SecondsPerHour;

        #endregion

        #region Private Fields

        private long ticks;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        public StopwatchTimeSpan( long ticks )
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
        public long Ticks
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
        public static int ToInt32( long ticks, int multiplier )
        {
            Double d = (Double) multiplier * ticks / TicksPerSecond;
            int int32 = (int) Math.Round( d );
            return int32;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static long ToInt64( long ticks, long multiplier )
        {
            Double d = (Double) multiplier * ticks / Stopwatch.Frequency;
            long int64 = (long) Math.Round( d );
            return int64;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static string ToString( long ticks, int scale )
        {
            long totalSeconds = ticks / TicksPerSecond;
            string fractionString = null;

            if (scale > 0)
            {
                string multiplierString = "1" + new string( '0', scale );
                long multiplier = long.Parse( multiplierString );
                long fractionTicks = ticks - (totalSeconds * TicksPerSecond);
                Double fraction = (Double) multiplier * fractionTicks / TicksPerSecond;
                fraction = Math.Round( fraction );
                long fractionInt64 = (long) fraction;
                if (fractionInt64 < multiplier)
                {
                    fractionString = string.Format( ".{0}", fractionInt64.ToString().PadLeft( scale, '0' ) );
                }
                else
                {
                    totalSeconds++;
                }
            }

            var sb = new StringBuilder();
            long days = ticks / TicksPerDay;

            if (days != 0)
            {
                totalSeconds -= SecondsPerDay * days;

                sb.Append( days );
                sb.Append( '.' );
            }

            long hours = totalSeconds / 3600;
            int seconds = (int) (totalSeconds - (hours * 3600));
            int minutes = seconds / 60;
            seconds -= minutes * 60;

            if (sb.Length > 0 || hours > 0)
            {
                sb.Append( hours.ToString().PadLeft( 2, '0' ) );
                sb.Append( ':' );
            }

            sb.Append( minutes.ToString().PadLeft( 2, '0' ) );
            sb.Append( ':' );
            sb.Append( seconds.ToString().PadLeft( 2, '0' ) );

            string s = string.Format( "{0}{1}", sb, fractionString );
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static long ToTicks( TimeSpan timeSpan )
        {
            return (long) (timeSpan.TotalSeconds * Stopwatch.Frequency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan( long elapsed )
        {
            long ticks = (long) ( elapsed*TicksPerTick );
            TimeSpan timeSpan = new TimeSpan( ticks );
            return timeSpan;
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public string ToString( int scale )
        {
            return ToString( this.ticks, scale );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString( this.ticks, 9 );
        }
    }
}