namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// 16 bit date type: 1990-01-01 - 2079-06-06
    /// </summary>
    [DataContract]
    public struct SmallDate : IComparable<SmallDate>, IEquatable<SmallDate>
    {
        [DataMember]
        private readonly ushort value;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DateTime MinDateTime = new DateTime(1900, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DateTime MaxDateTime = new DateTime(2079, 06, 07);

        /// <summary>
        /// 
        /// </summary>
        public static readonly SmallDate MinValue = new SmallDate(ushort.MinValue);

        /// <summary>
        /// 
        /// </summary>
        public static readonly SmallDate MaxValue = new SmallDate(ushort.MaxValue);

        private SmallDate(ushort value)
        {
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        public SmallDate(DateTime dateTime)
        {
            this.value = ToSmallDateValue(dateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public SmallDate(int year, int month, int day)
        {
            DateTime dateTime = new DateTime(year, month, day);
            this.value = ToSmallDateValue(dateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public static SmallDate Today
        {
            get
            {
                return new SmallDate(DateTime.Today);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Year
        {
            get
            {
                return ToDateTime(this.value).Year;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Month
        {
            get
            {
                return ToDateTime(this.value).Month;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Day
        {
            get
            {
                return ToDateTime(this.value).Day;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static explicit operator SmallDate(DateTime dateTime)
        {
            ushort value = ToSmallDateValue(dateTime);
            return new SmallDate(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Pure]
        public DateTime ToDateTime()
        {
            return ToDateTime(this.value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            DateTime dateTime = ToDateTime(this.value);
            return dateTime.ToShortDateString();
        }

        int IComparable<SmallDate>.CompareTo(SmallDate other)
        {
            return this.value.CompareTo(other.value);
        }

        private static DateTime ToDateTime(ushort value)
        {
            return MinDateTime.AddDays(value);
        }

        private static ushort ToSmallDateValue(DateTime dateTime)
        {
            Contract.Requires<ArgumentOutOfRangeException>(MinDateTime <= dateTime);
            Contract.Requires<ArgumentOutOfRangeException>(dateTime < MaxDateTime);

            TimeSpan timeSpan = dateTime - MinDateTime;
            double totalDays = timeSpan.TotalDays;
            ushort value = (ushort)totalDays;
            return value;
        }

        bool IEquatable<SmallDate>.Equals(SmallDate other)
        {
            return this.value == other.value;
        }
    }
}