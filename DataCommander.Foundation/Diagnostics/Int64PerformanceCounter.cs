﻿namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class Int64PerformanceCounter
    {
        private readonly string name;
        private readonly Func<long, string> toString;
        private long count;
        private long sum;
        private long min = long.MaxValue;
        private long max = long.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="toString"></param>
        public Int64PerformanceCounter( string name, Func<long, string> toString )
        {
            Contract.Requires( toString != null );

            this.name = name;
            this.toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Increment( long item )
        {
            Interlocked.Increment( ref this.count );
            Interlocked.Add( ref this.sum, item );

            while (true)
            {
                long min = this.min;
                if (item < min)
                {
                    long originalMin = Interlocked.CompareExchange( ref this.min, item, min );
                    if (originalMin == min)
                    {
                        break;
                    }
                    else
                    {
                        Thread.SpinWait( 1 );
                    }
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                long max = this.max;
                if (item > max)
                {
                    long originalMax = Interlocked.CompareExchange( ref this.max, item, max );
                    if (originalMax == max)
                    {
                        break;
                    }
                    else
                    {
                        Thread.SpinWait( 1 );
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Sum
        {
            get
            {
                return this.sum;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Min
        {
            get
            {
                return this.min;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long Max
        {
            get
            {
                return this.max;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToLogString()
        {
            return string.Format(
                "Int64PerformanceCounter '{0}'\r\ncount: {1}\r\nmin: {2}\r\navg: {3}\r\nmax: {4}\r\nsum: {5}",
                name,
                this.count,
                toString( this.min ),
                toString( (long)( (Double)this.Sum / this.Count ) ),
                toString( this.max ),
                toString( this.sum ) );
        }
    }
}