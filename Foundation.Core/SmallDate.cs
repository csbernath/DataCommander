using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Diagnostics.Contracts;

namespace Foundation
{
    /// <summary>
    /// 16 bit date type: 1900-01-01 - 2079-06-06
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct SmallDate : IComparable<SmallDate>, IEquatable<SmallDate>
    {
        private readonly ushort _value;

        public static readonly DateTime MinDateTime = new DateTime(1900, 1, 1);
        public static readonly DateTime MaxDateTime = new DateTime(2079, 06, 06);
        public static readonly SmallDate MinValue = new SmallDate(ushort.MinValue);
        public static readonly SmallDate MaxValue = new SmallDate(ushort.MaxValue);

        public SmallDate(ushort value)
        {
            _value = value;
        }

        public ushort Value => _value;

        public SmallDate(DateTime dateTime)
        {
            FoundationContract.Requires<ArgumentOutOfRangeException>(dateTime.TimeOfDay == TimeSpan.Zero);
            _value = ToSmallDateValue(dateTime);
        }

        public SmallDate(int year, int month, int day)
        {
            var dateTime = new DateTime(year, month, day);
            _value = ToSmallDateValue(dateTime);
        }

        public int Year => ToDateTime(_value).Year;
        public int Month => ToDateTime(_value).Month;
        public int Day => ToDateTime(_value).Day;

        public static explicit operator SmallDate(DateTime dateTime)
        {
            var value = ToSmallDateValue(dateTime);
            return new SmallDate(value);
        }

        public static SmallDate operator +(SmallDate smallDate, int value)
        {
            var result = smallDate._value + value;
            if (result < ushort.MinValue || ushort.MaxValue < result)
            {
                throw new OverflowException();
            }

            return new SmallDate((ushort) result);
        }

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

        public static bool operator <(SmallDate smallDate1, SmallDate smallDate2)
        {
            return smallDate1._value < smallDate2._value;
        }

        public static bool operator >(SmallDate smallDate1, SmallDate smallDate2)
        {
            return smallDate1._value > smallDate2._value;
        }

        [Pure]
        public bool In(SmallDateInterval interval)
        {
            return interval.Start._value <= _value && _value <= interval.End._value;
        }

        [Pure]
        public DateTime ToDateTime()
        {
            return ToDateTime(_value);
        }

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
        /// <param name="months"></param>
        /// <returns></returns>
        [Pure]
        public SmallDate AddMonths(int months)
        {
            var dateTime = ToDateTime();
            dateTime = dateTime.AddMonths(months);
            var result = ToSmallDateValue(dateTime);
            return new SmallDate(result);
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

        public string ToString(string format)
        {
            var dateTime = ToDateTime(_value);
            return dateTime.ToString(format);
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
            FoundationContract.Requires<ArgumentOutOfRangeException>(MinDateTime <= dateTime);
            FoundationContract.Requires<ArgumentOutOfRangeException>(dateTime <= MaxDateTime);

            var timeSpan = dateTime - MinDateTime;
            var totalDays = timeSpan.TotalDays;
            var value = (ushort) totalDays;
            return value;
        }

        bool IEquatable<SmallDate>.Equals(SmallDate other)
        {
            return _value == other._value;
        }

        public static bool operator ==(SmallDate x, SmallDate y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(SmallDate x, SmallDate y)
        {
            return !x.Equals(y);
        }

        internal string DebuggerDisplay
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