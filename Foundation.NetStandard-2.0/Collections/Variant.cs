using System;

namespace Foundation.Collections
{
    /// <summary>
    /// Discrete union
    /// </summary>
    public struct Variant<T1, T2, T3>
    {
        private Variant(object value, byte type)
        {
            Value = value;
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Variant(T1 value) : this(value, 0)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Variant(T2 value) : this(value, 1)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Variant(T3 value) : this(value, 2)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Variant<T1, T2, T3>(T1 value)
        {
            return new Variant<T1, T2, T3>(value, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// 
        /// </summary>
        public byte Type { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class VariantExtensions
    {
        private static Action<object> ToAction<T>(Action<T> source)
        {
            return value => source((T)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="variant"></param>
        /// <param name="action1"></param>
        /// <param name="action2"></param>
        /// <param name="action3"></param>
        public static void Action<T1, T2, T3>(this Variant<T1, T2, T3> variant, Action<T1> action1, Action<T2> action2, Action<T3> action3)
        {
            switch (variant.Type)
            {
                case 0:
                    var value1 = (T1)variant.Value;
                    action1(value1);
                    break;

                case 1:
                    var value2 = (T2)variant.Value;
                    action2(value2);
                    break;

                case 2:
                    var value3 = (T3)variant.Value;
                    action3(value3);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T0"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="variant"></param>
        /// <param name="function0"></param>
        /// <param name="function1"></param>
        /// <param name="function2"></param>
        /// <returns></returns>
        public static TResult Function<T0, T1, T2, TResult>(this Variant<T0, T1, T2> variant, Func<T0, TResult> function0, Func<T1, TResult> function1, Func<T2, TResult> function2)
        {
            var result = default(TResult);

            switch (variant.Type)
            {
                case 0:
                    var value0 = (T0)variant.Value;
                    result = function0(value0);
                    break;

                case 1:
                    var value1 = (T1)variant.Value;
                    result = function1(value1);
                    break;

                case 2:
                    var value2 = (T2)variant.Value;
                    result = function2(value2);
                    break;
            }

            return result;
        }
    }
}