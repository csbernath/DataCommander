using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Core
{
    /// <summary>
    /// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/DateOnly.cs
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct DateOnly : IComparable<DateOnly>
    {
        private readonly int _dayNumber;

        private const int MinDayNumber = 0;
        private const int MaxDayNumber = 3_652_058;

        private static int DayNumberFromDateTime(DateTime dt) => (int) (dt.Ticks / TimeSpan.TicksPerDay);

        private DateOnly(int dayNumber)
        {
            Assert.IsInRange((uint) dayNumber <= MaxDayNumber);
            _dayNumber = dayNumber;
        }

        public static DateOnly MinValue => new DateOnly(MinDayNumber);
        public static DateOnly MaxValue => new DateOnly(MaxDayNumber);

        public DateOnly(int year, int month, int day) => _dayNumber = DayNumberFromDateTime(new DateTime(year, month, day));

        [Pure]
        public DateOnly Next => AddDays(1);

        public DateOnly AddDays(int value)
        {
            int newDayNumber = _dayNumber + value;
            if ((uint) newDayNumber > MaxDayNumber)
            {
                ThrowOutOfRange();
            }

            return new DateOnly(newDayNumber);

            static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(value));
        }

        [Pure]
        public DateTime ToDateTime() => ToDateTime(_dayNumber);

        [Pure]
        private static DateTime ToDateTime(int dayNumber) => new DateTime(dayNumber * TimeSpan.TicksPerDay);
        
        public static bool operator ==(DateOnly left, DateOnly right) => left._dayNumber == right._dayNumber;
        public static bool operator !=(DateOnly left, DateOnly right) => left._dayNumber != right._dayNumber;
        public static bool operator >(DateOnly left, DateOnly right) => left._dayNumber > right._dayNumber;
        public static bool operator >=(DateOnly left, DateOnly right) => left._dayNumber >= right._dayNumber;
        public static bool operator <(DateOnly left, DateOnly right) => left._dayNumber < right._dayNumber;
        public static bool operator <=(DateOnly left, DateOnly right) => left._dayNumber <= right._dayNumber;

        public int CompareTo(DateOnly other) => _dayNumber.CompareTo(other._dayNumber);

        internal string DebuggerDisplay
        {
            get
            {
                string debuggerDisplay;

                if (_dayNumber == MinDayNumber)
                    debuggerDisplay = $"{ToDateTime():d}(min)";
                else if (_dayNumber == MaxDayNumber)
                    debuggerDisplay = $"{ToDateTime():d}(max)";
                else
                    debuggerDisplay = $"{ToDateTime():d}";

                return debuggerDisplay;
            }
        }
    }
}