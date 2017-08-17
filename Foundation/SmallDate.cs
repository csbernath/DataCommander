using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Foundation
{
    /// <summary>
    /// 16 bit date type: 1990-01-01 - 2079-06-06
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct SmallDate : IComparable<SmallDate>, IEquatable<SmallDate>
    {
        [DataMember] private readonly ushort _value;

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
            _value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        public SmallDate(DateTime dateTime)
        {
            _value = ToSmallDateValue(dateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public SmallDate(int year, int month, int day)
        {
            var dateTime = new DateTime(year, month, day);
            _value = ToSmallDateValue(dateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public static SmallDate Today(IDateTimeProvider dateTimeProvider)
        {
            var today = dateTimeProvider.Today();
            return new SmallDate(today);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Year => ToDateTime(_value).Year;

        /// <summary>
        /// 
        /// </summary>
        public int Month => ToDateTime(_value).Month;

        /// <summary>
        /// 
        /// </summary>
        public int Day => ToDateTime(_value).Day;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static explicit operator SmallDate(DateTime dateTime)
        {
            var value = ToSmallDateValue(dateTime);
            return new SmallDate(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smallDate"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SmallDate operator +(SmallDate smallDate, int value)
        {
            var result = smallDate._value + value;
            if (result < ushort.MinValue || ushort.MaxValue < result)
            {
                throw new OverflowException();
            }
            return new SmallDate((ushort) result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smallDate1"></param>
        /// <param name="smallDate2"></param>
        /// <returns></returns>
        public static int operator -(SmallDate smallDate1, SmallDate smallDate2)
        {
            return smallDate1._value - smallDate2._value;
        }

        public static bool operator <=(SmallDate smallDate1, SmallDate smallDate2)
        {
            return smallDate1._value <= smallDate2._value;
        }

        public static bool operator >=(SmallDate smallDate1, SmallDate smallDate2)
        {
            return smallDate1._value >= smallDate2._value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Pure]
        public DateTime ToDateTime()
        {
            return ToDateTime(_value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public SmallDate AddDays(short value)
        {
            var valueInt32 = _value + value;
            ushort valueUInt16;

            if (valueInt32 < ushort.MinValue)
                valueUInt16 = ushort.MinValue;
            else if (ushort.MaxValue < valueInt32)
                valueUInt16 = ushort.MaxValue;
            else
                valueUInt16 = (ushort) valueInt32;

            return new SmallDate(valueUInt16);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var dateTime = ToDateTime(_value);
            return dateTime.ToShortDateString();
        }

        int IComparable<SmallDate>.CompareTo(SmallDate other)
        {
            return _value.CompareTo(other._value);
        }

        private static DateTime ToDateTime(ushort value)
        {
            return MinDateTime.AddDays(value);
        }

        private static ushort ToSmallDateValue(DateTime dateTime)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentOutOfRangeException>(MinDateTime <= dateTime);
            Contract.Requires<ArgumentOutOfRangeException>(dateTime < MaxDateTime);
#endif

            var timeSpan = dateTime - MinDateTime;
            var totalDays = timeSpan.TotalDays;
            var value = (ushort) totalDays;
            return value;
        }

        bool IEquatable<SmallDate>.Equals(SmallDate other)
        {
            return _value == other._value;
        }

        private string DebuggerDisplay
        {
            get
            {
                string debuggerDisplay;

                if (_value == ushort.MinValue)
                    debuggerDisplay = $"{ToDateTime():d}(min)";
                else if (_value == ushort.MaxValue)
                    debuggerDisplay = $"{ToDateTime():d}(max)";
                else
                    debuggerDisplay = $"{ToDateTime():d}";

                return debuggerDisplay;
            }
        }
    }
}