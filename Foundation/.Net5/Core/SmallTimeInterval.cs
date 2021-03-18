using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Foundation.Assertions;

namespace Foundation.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct SmallTimeInterval : IEquatable<SmallTimeInterval>, IComparable<SmallTimeInterval>
    {
        public readonly SmallTime Start;
        public readonly SmallTime End;

        public SmallTimeInterval(SmallTime start, SmallTime end)
        {
            Assert.IsTrue(start.Value <= end.Value);

            Start = start;
            End = end;
        }

        [Pure]
        public bool Intersects(SmallTimeInterval other)
        {
            var start = Math.Max(Start.Value, other.Start.Value);
            var end = Math.Min(End.Value, other.End.Value);
            var intersects = start < end;
            return intersects;
        }

        [Pure]
        public static bool Intersects(IEnumerable<SmallTimeInterval> egyik, IEnumerable<SmallTimeInterval> masik)
        {
            var rendezettEgyik = egyik.OrderBy(i => i.Start).ToList();
            var rendezettMasik = masik.OrderBy(i => i.Start).ToList();
            var intersects = false;

            foreach (var egyikIdoszak in rendezettEgyik)
            foreach (var masikIdoszak in rendezettMasik)
                if (egyikIdoszak.Intersects(masikIdoszak))
                {
                    intersects = true;
                    break;
                }

            return intersects;
        }

        public bool Equals(SmallTimeInterval other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public int CompareTo(SmallTimeInterval other)
        {
            var result = Start.Value.CompareTo(other.Start.Value);
            if (result == 0)
                result = End.Value.CompareTo(other.End.Value);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SmallTimeInterval && Equals((SmallTimeInterval) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }

        private string DebuggerDisplay => $"{Start.DebuggerDisplay}-{End.DebuggerDisplay}";
    }
}