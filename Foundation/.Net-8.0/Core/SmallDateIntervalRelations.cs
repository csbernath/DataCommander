using System;

namespace Foundation.Core;

public static class SmallDateIntervalRelations
{
    public static bool Precedes(this SmallDateInterval x, SmallDateInterval y) => x.End.Next < y.Start;
    public static bool Meets(this SmallDateInterval x, SmallDateInterval y) => x.End.Next == y.Start;
    public static bool OverlapsWith(this SmallDateInterval x, SmallDateInterval y) => x.Start < y.Start && y.Start <= x.End && x.End < y.End;
    public static bool Starts(this SmallDateInterval x, SmallDateInterval y) => x.Start == y.Start && x.End < y.End;
    public static bool During(this SmallDateInterval x, SmallDateInterval y) => y.Start < x.Start && x.End < y.End;
    public static bool Finishes(this SmallDateInterval x, SmallDateInterval y) => y.Start < x.Start && x.End == y.End;
    public static bool IsPrecededBy(this SmallDateInterval x, SmallDateInterval y) => y.Precedes(x);
    public static bool IsMetBy(this SmallDateInterval x, SmallDateInterval y) => y.Meets(x);
    public static bool IsOverlappedBy(this SmallDateInterval x, SmallDateInterval y) => y.OverlapsWith(x);
    public static bool IsStartedBy(this SmallDateInterval x, SmallDateInterval y) => y.Starts(x);
    public static bool Contains(this SmallDateInterval x, SmallDateInterval y) => y.During(x);
    public static bool IsFinishedBy(this SmallDateInterval x, SmallDateInterval y) => y.Finishes(x);
    public static bool IsEqualTo(this SmallDateInterval x, SmallDateInterval y) => x.Start == y.Start && x.End == y.End;

    public static TemporalIntervalRelation GetTemporalIntervalRelation(this SmallDateInterval x, SmallDateInterval y)
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