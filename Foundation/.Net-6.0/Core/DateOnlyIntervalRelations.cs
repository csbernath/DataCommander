using System;

namespace Foundation.Core
{
    public static class DateOnlyIntervalRelations
    {
        public static bool Precedes(this DateOnlyInterval x, DateOnlyInterval y) => x.End.Next < y.Start;
        public static bool Meets(this DateOnlyInterval x, DateOnlyInterval y) => x.End.Next == y.Start;
        public static bool OverlapsWith(this DateOnlyInterval x, DateOnlyInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
        public static bool Starts(this DateOnlyInterval x, DateOnlyInterval y) => x.Start == y.Start && x.End < y.End;
        public static bool During(this DateOnlyInterval x, DateOnlyInterval y) => y.Start < x.Start && x.End < y.End;
        public static bool Finishes(this DateOnlyInterval x, DateOnlyInterval y) => y.Start < x.Start && x.End == y.End;
        public static bool IsPrecededBy(this DateOnlyInterval x, DateOnlyInterval y) => Precedes(y, x);
        public static bool IsMetBy(this DateOnlyInterval x, DateOnlyInterval y) => Meets(y, x);
        public static bool IsOverlappedBy(this DateOnlyInterval x, DateOnlyInterval y) => OverlapsWith(y, x);
        public static bool IsStartedBy(this DateOnlyInterval x, DateOnlyInterval y) => Starts(y, x);
        public static bool Contains(this DateOnlyInterval x, DateOnlyInterval y) => During(y, x);
        public static bool IsFinishedBy(this DateOnlyInterval x, DateOnlyInterval y) => Finishes(y, x);
        public static bool IsEqualTo(this DateOnlyInterval x, DateOnlyInterval y) => x.Start == y.Start && x.End == y.End;

        public static TemporalIntervalRelation GetTemporalIntervalRelation(this DateOnlyInterval x, DateOnlyInterval y)
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