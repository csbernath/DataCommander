using System;
using System.Diagnostics;

namespace Foundation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct MinuteSpan : IComparable<MinuteSpan>
    {
        private readonly int _value;

        public static readonly MinuteSpan Zero = new MinuteSpan(0);

        public MinuteSpan(int value) => _value = value;

        public int TotalMinutes => _value;

        public static bool operator ==(MinuteSpan x, MinuteSpan y) => x._value == y._value;
        public static bool operator !=(MinuteSpan x, MinuteSpan y) => !(x == y);
        public static bool operator <(MinuteSpan x, MinuteSpan y) => x._value < y._value;
        public static bool operator <=(MinuteSpan x, MinuteSpan y) => x._value <= y._value;
        public static bool operator >(MinuteSpan x, MinuteSpan y) => x._value > y._value;
        public static bool operator >=(MinuteSpan x, MinuteSpan y) => x._value >= y._value;
        public static MinuteSpan operator +(MinuteSpan x, MinuteSpan y) => new MinuteSpan(x._value + y._value);
        public static MinuteSpan operator -(MinuteSpan x, MinuteSpan y) => new MinuteSpan(x._value - y._value);

        public static MinuteSpan FromMinutes(int minutes) => new MinuteSpan(minutes);

        public static MinuteSpan FromHours(double hours)
        {
            var minutes = (int) (DateTimeConstants.MinutesPerHour * hours);
            return new MinuteSpan(minutes);
        }

        public int CompareTo(MinuteSpan other) => _value.CompareTo(other._value);

        public string DebuggerDisplay
        {
            get
            {
                var hours = _value / DateTimeConstants.MinutesPerHour;
                var minutes = _value - DateTimeConstants.MinutesPerHour * hours;

                var debuggerDisplay = $"{hours}:{minutes:D2}";

                return debuggerDisplay;
            }
        }
    }
}