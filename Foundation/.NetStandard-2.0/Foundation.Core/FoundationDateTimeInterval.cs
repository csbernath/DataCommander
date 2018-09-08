using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Diagnostics.Contracts;

namespace Foundation
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct FoundationDateTimeInterval
    {
        public readonly DateTime Start;
        public readonly DateTime End;

        public FoundationDateTimeInterval(DateTime start, DateTime end)
        {
            FoundationContract.Requires<ArgumentException>(start <= end);
            Start = start;
            End = end;
        }

        [Pure]
        public bool Contains(FoundationDateTimeInterval other) => Start <= other.Start && other.End <= End;

        [Pure]
        public FoundationDateTimeInterval? Intersect(FoundationDateTimeInterval other)
        {
            var start = ElementPair.Max(Start, other.Start);
            var end = ElementPair.Min(End, other.End);
            var intersects = start < end;
            return intersects
                ? new FoundationDateTimeInterval(start, end)
                : (FoundationDateTimeInterval?) null;
        }

        [Pure]
        public bool Intersects(FoundationDateTimeInterval other)
        {
            var start = ElementPair.Max(Start, other.Start);
            var end = ElementPair.Min(End, other.End);
            var intersects = start < end;
            return intersects;
        }

        [Pure]
        public TimeSpan GetLength()
        {
            var length = End - Start;
            return length;
        }

        private static string ToString(DateTime dateTime) => dateTime.ToString("yyyy.MM.dd. HH:mm:ss");

        private string DebuggerDisplay => $"{ToString(Start)}-{ToString(End)}";
    }
}