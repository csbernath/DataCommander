using System;

namespace Foundation.Core;

public static class DateOnlyIntervalRelations
{
    extension(DateOnlyInterval x)
    {
        public bool Precedes(DateOnlyInterval y) => x.End.AddDays(1) < y.Start;
        public bool Meets(DateOnlyInterval y) => x.End.AddDays(1) == y.Start;
        public bool OverlapsWith(DateOnlyInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
        public bool Starts(DateOnlyInterval y) => x.Start == y.Start && x.End < y.End;
        public bool During(DateOnlyInterval y) => y.Start < x.Start && x.End < y.End;
        public bool Finishes(DateOnlyInterval y) => y.Start < x.Start && x.End == y.End;
        public bool IsPrecededBy(DateOnlyInterval y) => Precedes(y, x);
        public bool IsMetBy(DateOnlyInterval y) => Meets(y, x);
        public bool IsOverlappedBy(DateOnlyInterval y) => OverlapsWith(y, x);
        public bool IsStartedBy(DateOnlyInterval y) => Starts(y, x);
        public bool Contains(DateOnlyInterval y) => During(y, x);
        public bool IsFinishedBy(DateOnlyInterval y) => Finishes(y, x);
        public bool IsEqualTo(DateOnlyInterval y) => x.Start == y.Start && x.End == y.End;

        public TemporalIntervalRelation GetTemporalIntervalRelation(DateOnlyInterval y)
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