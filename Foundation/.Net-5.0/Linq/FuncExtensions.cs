using System;

namespace Foundation.Linq;

public static class FuncExtensions
{
    public static Action AsAction<TResult>(this Func<TResult> func) => () => func();
    public static Action<T> AsAction<T, TResult>(this Func<T, TResult> func) => t => func(t);
    public static Action<T1, T2> AsAction<T1, T2, TResult>(this Func<T1, T2, TResult> func) => (t1, t2) => func(t1, t2);
    public static Action<T1, T2, T3> AsAction<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func) => (t1, t2, t3) => func(t1, t2, t3);

    public static Action<T1, T2, T3, T4> AsAction<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func) =>
        (t1, t2, t3, t4) => func(t1, t2, t3, t4);
}