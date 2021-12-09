using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Foundation.Assertions;

namespace Foundation.Core
{
    public readonly struct DateOnlyInterval
    {
        public readonly DateOnly Start;
        public readonly DateOnly End;

        public DateOnlyInterval(DateOnly start, DateOnly end)
        {
            Assert.IsTrue(start <= end);
            Start = start;
            End = end;
        }

        [Pure]
        public bool Contains(DateOnly date)
        {
            var contains = Start <= date && date <= End;
            return contains;
        }

        [Pure]
        public bool Contains(DateOnlyInterval other)
        {
            var contains = Start <= other.Start && other.End <= End;
            return contains;
        }

        [Pure]
        public DateOnlyInterval? Intersect(DateOnlyInterval other)
        {
            var start = ElementPair.Max(Start, other.Start);
            var end = ElementPair.Min(End, other.End);
            var intersects = start <= end;
            return intersects
                ? new DateOnlyInterval(start, end)
                : (DateOnlyInterval?) null;
        }

        [Pure]
        public bool Intersects(DateOnlyInterval other)
        {
            var start = ElementPair.Max(Start, other.Start);
            var end = ElementPair.Min(End, other.End);
            var intersects = start <= end;
            return intersects;
        }

        //[Pure]
        //public int GetLength()
        //{
        //    var length = End - Start + 1;
        //    return length;
        //}

        [Pure]
        public IEnumerable<DateOnly> GetDates()
        {
            for (var date = Start; date <= End; date = date.AddDays(1))
                yield return date;
        }

        private string DebuggerDisplay => $"{Start.DebuggerDisplay}-{End.DebuggerDisplay}";
    }
}