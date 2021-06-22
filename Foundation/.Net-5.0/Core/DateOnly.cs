using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Foundation.Core
{
    /// <summary>
    /// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/DateOnly.cs
    /// </summary>
    public readonly struct DateOnly
    {
        private readonly int _dayNumber;

        private const int MaxDayNumber = 3_652_058;

        private static int DayNumberFromDateTime(DateTime dt) => (int) (dt.Ticks / TimeSpan.TicksPerDay);

        private DateOnly(int dayNumber)
        {
            Debug.Assert((uint) dayNumber <= MaxDayNumber);
            _dayNumber = dayNumber;
        }

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

        public static bool operator ==(DateOnly left, DateOnly right) => left._dayNumber == right._dayNumber;
        public static bool operator !=(DateOnly left, DateOnly right) => left._dayNumber != right._dayNumber;
        public static bool operator >(DateOnly left, DateOnly right) => left._dayNumber > right._dayNumber;
        public static bool operator >=(DateOnly left, DateOnly right) => left._dayNumber >= right._dayNumber;
        public static bool operator <(DateOnly left, DateOnly right) => left._dayNumber < right._dayNumber;
        public static bool operator <=(DateOnly left, DateOnly right) => left._dayNumber <= right._dayNumber;
    }
}