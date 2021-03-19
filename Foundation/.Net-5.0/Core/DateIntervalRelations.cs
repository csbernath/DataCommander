using System;

namespace Foundation.Core
{
    public static class DateIntervalRelations
    {
        public static bool Precedes(this DateInterval x, DateInterval y) => x.End.Next < y.Start;
        public static bool Meets(this DateInterval x, DateInterval y) => x.End.Next == y.Start;
        public static bool OverlapsWith(this DateInterval x, DateInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
        public static bool Starts(this DateInterval x, DateInterval y) => x.Start == y.Start && x.End < y.End;
        public static bool During(this DateInterval x, DateInterval y) => y.Start < x.Start && x.End < y.End;
        public static bool Finishes(this DateInterval x, DateInterval y) => y.Start < x.Start && x.End == y.End;
        public static bool IsPrecededBy(this DateInterval x, DateInterval y) => Precedes(y, x);
        public static bool IsMetBy(this DateInterval x, DateInterval y) => Meets(y, x);
        public static bool IsOverlappedBy(this DateInterval x, DateInterval y) => OverlapsWith(y, x);
        public static bool IsStartedBy(this DateInterval x, DateInterval y) => Starts(y, x);
        public static bool Contains(this DateInterval x, DateInterval y) => During(y, x);
        public static bool IsFinishedBy(this DateInterval x, DateInterval y) => Finishes(y, x);
        public static bool IsEqualTo(this DateInterval x, DateInterval y) => x.Start == y.Start && x.End == y.End;

        public static TemporalIntervalRelation GetTemporalIntervalRelation(this DateInterval x, DateInterval y)
        {
            TemporalIntervalRelation relation;

            if (Precedes(x, y))
                relation = TemporalIntervalRelation.Precedes;
            else if (Meets(x, y))
                relation = TemporalIntervalRelation.Meets;
            else if (OverlapsWith(x, y))
                relation = TemporalIntervalRelation.OverlapsWith;
            else if (Starts(x, y))
                relation = TemporalIntervalRelation.Starts;
            else if (During(x, y))
                relation = TemporalIntervalRelation.During;
            else if (Finishes(x, y))
                relation = TemporalIntervalRelation.Finishes;
            else if (IsPrecededBy(x, y))
                relation = TemporalIntervalRelation.IsPrecededBy;
            else if (IsMetBy(x, y))
                relation = TemporalIntervalRelation.IsMetBy;
            else if (IsOverlappedBy(x, y))
                relation = TemporalIntervalRelation.IsOverlappedBy;
            else if (IsStartedBy(x, y))
                relation = TemporalIntervalRelation.IsStartedBy;
            else if (Contains(x, y))
                relation = TemporalIntervalRelation.Contains;
            else if (IsFinishedBy(x, y))
                relation = TemporalIntervalRelation.IsFinishedBy;
            else if (IsEqualTo(x, y))
                relation = TemporalIntervalRelation.IsEqualTo;
            else
                throw new InvalidOperationException();

            return relation;
        }
    }
}