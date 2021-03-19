//using System.Collections.Generic;
//using Foundation.Assertions;

//namespace Foundation
//{
//    public sealed class TimeInterval<T>
//    {
//        public readonly T Start;
//        public readonly T End;

//        public TimeInterval(T start, T end)
//        {
//            Start = start;
//            End = end;
//        }
//    }

//    public interface ITemporalIntervalRelations
//    {
//        bool Before(ITemporalInterval x, ITemporalInterval y);
//        bool Equal(ITemporalInterval other);
//        bool Meets(ITemporalInterval other);
//        bool Overlaps(ITemporalInterval other);
//        bool During(ITemporalInterval other);
//        bool Starts(ITemporalInterval other);
//        bool Finishes(ITemporalInterval other);
//    }

//    public static class ITemporalIntervalExtensions
//    {
//        public static bool After(this ITemporalInterval x, ITemporalInterval y) => y.Before(x);
//        public static bool Contains(this ITemporalInterval x, ITemporalInterval y) => y.During(x);
//        public static bool OverlappedBy(this ITemporalInterval x, ITemporalInterval y) => y.Overlaps(x);
//        public static bool MetBy(this ITemporalInterval x, ITemporalInterval y) => y.Meets(x);
//        public static bool StartedBy(this ITemporalInterval x, ITemporalInterval y) => y.Starts(x);
//        public static bool FinishedBy(this ITemporalInterval x, ITemporalInterval y) => y.Finishes(x);
//    }

//    public sealed class TemporalIntervalRelations<T> : ITemporalInterval
//    {
//        private readonly ITemporalIntervalStartEnd<T> _x;
//        private readonly IComparer<T> _comparer;

//        public TemporalIntervalRelations(ITemporalIntervalStartEnd<T> x, IComparer<T> comparer)
//        {
//            Assert.IsNotNull(x);
//            Assert.IsNotNull(comparer);
//            _x = x;
//            _comparer = comparer;
//        }

//        public bool Before(ITemporalInterval y)
//        {
//            return _comparer.Compare(_x.End,y.
//        }

//        public bool Equal(ITemporalInterval other)
//        {
//            throw new System.NotImplementedException();
//        }

//        public bool Meets(ITemporalInterval other)
//        {
//            throw new System.NotImplementedException();
//        }

//        public bool Overlaps(ITemporalInterval other)
//        {
//            throw new System.NotImplementedException();
//        }

//        public bool During(ITemporalInterval other)
//        {
//            throw new System.NotImplementedException();
//        }

//        public bool Starts(ITemporalInterval other)
//        {
//            throw new System.NotImplementedException();
//        }

//        public bool Finishes(ITemporalInterval other)
//        {
//            throw new System.NotImplementedException();
//        }
//    }
//}