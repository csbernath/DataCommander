using System;

namespace Foundation.Core
{
    public static class SmallDateIntervalRelations
    {
        public static bool Before(this SmallDateInterval x, SmallDateInterval y) => x.End < y.Start;
        public static bool Meets(this SmallDateInterval x, SmallDateInterval y) => x.End.AddDays(1) == y.Start;
        public static bool Overlaps(this SmallDateInterval x, SmallDateInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
        public static bool Starts(this SmallDateInterval x, SmallDateInterval y) => x.Start == y.Start && x.End < y.End;
        public static bool During(this SmallDateInterval x, SmallDateInterval y) => y.Start < x.Start && x.End < y.End;
        public static bool Finishes(this SmallDateInterval x, SmallDateInterval y) => y.Start < x.Start && x.End == y.End;

        public static bool Equal(this SmallDateInterval x, SmallDateInterval y) => x.Start == y.Start && x.End == y.End;

        public static bool After(this SmallDateInterval x, SmallDateInterval y) => y.Before(x);
        public static bool MetBy(this SmallDateInterval x, SmallDateInterval y) => y.Meets(x);
        public static bool OverlappedBy(this SmallDateInterval x, SmallDateInterval y) => y.Overlaps(x);
        public static bool StartedBy(this SmallDateInterval x, SmallDateInterval y) => y.Starts(x);
        public static bool Contains(this SmallDateInterval x, SmallDateInterval y) => y.During(x);
        public static bool FinishedBy(this SmallDateInterval x, SmallDateInterval y) => y.Finishes(x);

        public static TemporalIntervalRelation GetTemporalIntervalRelation(this SmallDateInterval x, SmallDateInterval y)
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