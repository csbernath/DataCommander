using System;
using Foundation.Assertions;

namespace Foundation
{
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
        public static Func<TX, TZ> Compose<TX, TY, TZ>(Func<TX, TY> func1, Func<TY, TZ> func2)
        {
            var composition = new UnaryFunctionComposition<TX, TY, TZ>(func1, func2);
            return composition.Evaluate;
        }

        private sealed class UnaryFunctionComposition<TX, TY, TZ>
        {
            private readonly Func<TX, TY> _func1;
            private readonly Func<TY, TZ> _func2;

            public UnaryFunctionComposition(Func<TX, TY> func1, Func<TY, TZ> func2)
            {
                Assert.IsNotNull(func1);
                Assert.IsNotNull(func2);

                _func1 = func1;
                _func2 = func2;
            }

            public TZ Evaluate(TX x)
            {
                var y = _func1(x);
                var z = _func2(y);
                return z;
            }
        }
    }
}