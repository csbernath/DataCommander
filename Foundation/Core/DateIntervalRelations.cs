using System;

namespace Foundation.Core;

public static class DateIntervalRelations
{
    extension(DateInterval x)
    {
        public bool Precedes(DateInterval y) => x.End.Next < y.Start;
        public bool Meets(DateInterval y) => x.End.Next == y.Start;
        public bool OverlapsWith(DateInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
        public bool Starts(DateInterval y) => x.Start == y.Start && x.End < y.End;
        public bool During(DateInterval y) => y.Start < x.Start && x.End < y.End;
        public bool Finishes(DateInterval y) => y.Start < x.Start && x.End == y.End;
        public bool IsPrecededBy(DateInterval y) => Precedes(y, x);
        public bool IsMetBy(DateInterval y) => Meets(y, x);
        public bool IsOverlappedBy(DateInterval y) => OverlapsWith(y, x);
        public bool IsStartedBy(DateInterval y) => Starts(y, x);
        public bool Contains(DateInterval y) => During(y, x);
        public bool IsFinishedBy(DateInterval y) => Finishes(y, x);
        public bool IsEqualTo(DateInterval y) => x.Start == y.Start && x.End == y.End;

        public TemporalIntervalRelation GetTemporalIntervalRelation(DateInterval y)
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