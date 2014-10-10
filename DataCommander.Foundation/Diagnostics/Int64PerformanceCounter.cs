namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class Int64PerformanceCounter
    {
        private readonly String name;
        private readonly Func<Int64, String> toString;
        private Int64 count;
        private Int64 sum;
        private Int64 min = Int64.MaxValue;
        private Int64 max = Int64.MinValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="toString"></param>
        public Int64PerformanceCounter( String name, Func<Int64, String> toString )
        {
            Contract.Requires( toString != null );

            this.name = name;
            this.toString = toString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Increment( Int64 item )
        {
            Interlocked.Increment( ref this.count );
            Interlocked.Add( ref this.sum, item );

            while (true)
            {
                Int64 min = this.min;
                if (item < min)
                {
                    Int64 originalMin = Interlocked.CompareExchange( ref this.min, item, min );
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
                Int64 max = this.max;
                if (item > max)
                {
                    Int64 originalMax = Interlocked.CompareExchange( ref this.max, item, max );
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
        public Int64 Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Sum
        {
            get
            {
                return this.sum;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Min
        {
            get
            {
                return this.min;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Max
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
        public String ToLogString()
        {
            return String.Format(
                "Int64PerformanceCounter '{0}'\r\ncount: {1}\r\nmin: {2}\r\navg: {3}\r\nmax: {4}\r\nsum: {5}",
                name,
                this.count,
                toString( this.min ),
                toString( (Int64)( (Double)this.Sum / this.Count ) ),
                toString( this.max ),
                toString( this.sum ) );
        }
    }
}