using System;

namespace Foundation.Core;

public static class SmallDateIntervalRelations
{
    extension(SmallDateInterval x)
    {
        public bool Precedes(SmallDateInterval y) => x.End.Next < y.Start;
        public bool Meets(SmallDateInterval y) => x.End.Next == y.Start;
        public bool OverlapsWith(SmallDateInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
        public bool Starts(SmallDateInterval y) => x.Start == y.Start && x.End < y.End;
        public bool During(SmallDateInterval y) => y.Start < x.Start && x.End < y.End;
        public bool Finishes(SmallDateInterval y) => y.Start < x.Start && x.End == y.End;
        public bool IsPrecededBy(SmallDateInterval y) => y.Precedes(x);
        public bool IsMetBy(SmallDateInterval y) => y.Meets(x);
        public bool IsOverlappedBy(SmallDateInterval y) => y.OverlapsWith(x);
        public bool IsStartedBy(SmallDateInterval y) => y.Starts(x);
        public bool Contains(SmallDateInterval y) => y.During(x);
        public bool IsFinishedBy(SmallDateInterval y) => y.Finishes(x);
        public bool IsEqualTo(SmallDateInterval y) => x.Start == y.Start && x.End == y.End;

        public TemporalIntervalRelation GetTemporalIntervalRelation(SmallDateInterval y)
        {
            TemporalIntervalRelation relation;

            if (x.Precedes(y))
                relation = TemporalIntervalRelation.Precedes;
            else if (x.Meets(y))
                relation = TemporalIntervalRelation.Meets;
            else if (x.OverlapsWith(y))
                relation = TemporalIntervalRelation.OverlapsWith;
            else if (x.Starts(y))
                relation = TemporalIntervalRelation.Starts;
            else if (x.During(y))
                relation = TemporalIntervalRelation.During;
            else if (x.Finishes(y))
                relation = TemporalIntervalRelation.Finishes;
            else if (x.IsPrecededBy(y))
                relation = TemporalIntervalRelation.IsPrecededBy;
            else if (x.IsMetBy(y))
                relation = TemporalIntervalRelation.IsMetBy;
            else if (x.IsOverlappedBy(y))
                relation = TemporalIntervalRelation.IsOverlappedBy;
            else if (x.IsStartedBy(y))
                relation = TemporalIntervalRelation.IsStartedBy;
            else if (Contains(x, y))
                relation = TemporalIntervalRelation.Contains;
            else if (x.IsFinishedBy(y))
                relation = TemporalIntervalRelation.IsFinishedBy;
            else if (x.IsEqualTo(y))
                relation = TemporalIntervalRelation.IsEqualTo;
            else
                throw new InvalidOperationException();

            return relation;
        }
    }
}