using System;

namespace Foundation
{
    public static class SmallTimeIntervalRelations
    {
        public static bool Before(this SmallTimeInterval x, SmallTimeInterval y) => x.End < y.Start;
        public static bool Meets(this SmallTimeInterval x, SmallTimeInterval y) => x.End == y.Start;
        public static bool Overlaps(this SmallTimeInterval x, SmallTimeInterval y) => x.Start < y.Start && y.Start < x.End && x.End < y.End;
        public static bool Starts(this SmallTimeInterval x, SmallTimeInterval y) => x.Start == y.Start && x.End < y.End;
        public static bool During(this SmallTimeInterval x, SmallTimeInterval y) => y.Start < x.Start && x.End < y.End;
        public static bool Finishes(this SmallTimeInterval x, SmallTimeInterval y) => y.Start < x.Start && x.End == y.End;

        public static bool Equal(this SmallTimeInterval x, SmallTimeInterval y) => x.Start == y.Start && x.End == y.End;

        public static bool After(this SmallTimeInterval x, SmallTimeInterval y) => y.Before(x);
        public static bool MetBy(this SmallTimeInterval x, SmallTimeInterval y) => y.Meets(x);
        public static bool OverlappedBy(this SmallTimeInterval x, SmallTimeInterval y) => y.Overlaps(x);
        public static bool StartedBy(this SmallTimeInterval x, SmallTimeInterval y) => y.Starts(x);
        public static bool Contains(this SmallTimeInterval x, SmallTimeInterval y) => y.During(x);
        public static bool FinishedBy(this SmallTimeInterval x, SmallTimeInterval y) => y.Finishes(x);

        public static TemporalIntervalRelation GetTemporalIntervalRelation(this SmallTimeInterval x, SmallTimeInterval y)
        {
            TemporalIntervalRelation relation;

            if (x.Before(y))
                relation = TemporalIntervalRelation.Before;
            else if (x.Meets(y))
                relation = TemporalIntervalRelation.Meets;
            else if (x.Overlaps(y))
                relation = TemporalIntervalRelation.Overlaps;
            else if (x.Starts(y))
                relation = TemporalIntervalRelation.Starts;
            else if (x.During(y))
                relation = TemporalIntervalRelation.During;
            else if (x.Finishes(y))
                relation = TemporalIntervalRelation.Finishes;
            else if (x.Equal(y))
                relation = TemporalIntervalRelation.Equal;
            else if (x.After(y))
                relation = TemporalIntervalRelation.After;
            else if (x.MetBy(y))
                relation = TemporalIntervalRelation.MetBy;
            else if (x.OverlappedBy(y))
                relation = TemporalIntervalRelation.OverlappedBy;
            else if (x.StartedBy(y))
                relation = TemporalIntervalRelation.StartedBy;
            else if (x.Contains(y))
                relation = TemporalIntervalRelation.Contains;
            else if (x.FinishedBy(y))
                relation = TemporalIntervalRelation.FinishedBy;
            else
                throw new InvalidOperationException();

            return relation;
        }
    }
}