namespace DataCommander.Foundation.Linq
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class FuncExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Action AsAction<TResult>(this Func<TResult> func)
        {
            return () => func();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Action<T> AsAction<T, TResult>(this Func<T, TResult> func)
        {
            return t => func(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Action<T1, T2> AsAction<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            return (t1, t2) => func(t1, t2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Action<T1, T2, T3> AsAction<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            return (t1, t2, t3) => func(t1, t2, t3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Action<T1, T2, T3, T4> AsAction<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
        {
            return (t1, t2, t3, t4) => func(t1, t2, t3, t4);
        }
    }
}