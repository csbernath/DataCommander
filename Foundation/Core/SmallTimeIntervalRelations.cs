using System;

namespace Foundation.Core;

public static class SmallTimeIntervalRelations
{
    extension(SmallTimeInterval x)
    {
        public bool Before(SmallTimeInterval y) => x.End < y.Start;
        public bool Meets(SmallTimeInterval y) => x.End == y.Start;
        public bool OverlapsWith(SmallTimeInterval y) => x.Start < y.Start && y.Start < x.End && x.End < y.End;
        public bool Starts(SmallTimeInterval y) => x.Start == y.Start && x.End < y.End;
        public bool During(SmallTimeInterval y) => y.Start < x.Start && x.End < y.End;
        public bool Finishes(SmallTimeInterval y) => y.Start < x.Start && x.End == y.End;

        public bool Equal(SmallTimeInterval y) => x.Start == y.Start && x.End == y.End;

        public bool After(SmallTimeInterval y) => y.Before(x);
        public bool MetBy(SmallTimeInterval y) => y.Meets(x);
        public bool OverlappedBy(SmallTimeInterval y) => y.OverlapsWith(x);
        public bool StartedBy(SmallTimeInterval y) => y.Starts(x);
        public bool Contains(SmallTimeInterval y) => y.During(x);
        public bool FinishedBy(SmallTimeInterval y) => y.Finishes(x);

        public TemporalIntervalRelation GetTemporalIntervalRelation(SmallTimeInterval y)
        {
            TemporalIntervalRelation relation;

            if (x.Before(y))
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
            else if (x.Equal(y))
                relation = TemporalIntervalRelation.IsEqualTo;
            else if (x.After(y))
                relation = TemporalIntervalRelation.IsPrecededBy;
            else if (x.MetBy(y))
                relation = TemporalIntervalRelation.IsMetBy;
            else if (x.OverlappedBy(y))
                relation = TemporalIntervalRelation.IsOverlappedBy;
            else if (x.StartedBy(y))
                relation = TemporalIntervalRelation.IsStartedBy;
            else if (x.Contains(y))
                relation = TemporalIntervalRelation.Contains;
            else if (x.FinishedBy(y))
                relation = TemporalIntervalRelation.IsFinishedBy;
            else
                throw new InvalidOperationException();

            return relation;
        }
    }
}