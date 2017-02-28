namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class FunctionComposition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <typeparam name="Y"></typeparam>
        /// <typeparam name="Z"></typeparam>
        /// <param name="func1"></param>
        /// <param name="func2"></param>
        /// <returns></returns>
        public static Func<X, Z> Compose<X, Y, Z>(Func<X, Y> func1, Func<Y, Z> func2)
        {
            var composition = new UnaryFunctionComposition<X, Y, Z>(func1, func2);
            return composition.Evaluate;
        }

        private sealed class UnaryFunctionComposition<X, Y, Z>
        {
            private readonly Func<X, Y> func1;
            private readonly Func<Y, Z> func2;

            public UnaryFunctionComposition(Func<X, Y> func1, Func<Y, Z> func2)
            {
                Contract.Requires(func1 != null);
                Contract.Requires(func2 != null);

                this.func1 = func1;
                this.func2 = func2;
            }

            public Z Evaluate(X x)
            {
                var y = this.func1(x);
                var z = this.func2(y);
                return z;
            }
        }
    }
}