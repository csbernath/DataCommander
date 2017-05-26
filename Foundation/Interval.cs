//namespace DataCommander.Foundation
//{
//    using System;
//    using System.Diagnostics.Contracts;

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public struct Interval<T, TIntervalRelation> where TIntervalRelation : IIntervalRelation<T>, new()
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public static readonly TIntervalRelation IntervalRelation = new TIntervalRelation();

//        /// <summary>
//        /// 
//        /// </summary>
//        public readonly T Left;

//        /// <summary>
//        /// 
//        /// </summary>
//        public readonly T Right;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="left"></param>
//        /// <param name="right"></param>
//        public Interval(T left, T right)
//        {
//            //Contract.Requires<ArgumentException>(IntervalRelation.IsValid(left, right));

//            this.Left = left;
//            this.Right = right;
//        }
//    }
//}