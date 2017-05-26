using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public static class Selection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static ArgumentEqualsSelection<TArgument> CreateArgumentEqualsSelection<TArgument>(TArgument argument) where TArgument : IEquatable<TArgument>
        {
            return new ArgumentEqualsSelection<TArgument>(argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static ArgumentIsSelection<TArgument> CreateArgumentIsSelection<TArgument>(TArgument argument) where TArgument : class
        {
            return new ArgumentIsSelection<TArgument>(argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeIsSelection CreateTypeIsSelection(Type type)
        {
            return new TypeIsSelection(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selections"></param>
        /// <returns></returns>
        public static int Select(Func<bool>[] selections)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(selections != null);
#endif

            var selectedIndex = -1;

            for (var i = 0; i < selections.Length; ++i)
            {
                var selection = selections[i];
                var selected = selection();

                if (selected)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return selectedIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <typeparam name="TActionArgument"></typeparam>
        /// <param name="argument"></param>
        /// <param name="actionArgument"></param>
        /// <param name="getAction"></param>
        public static void Select<TArgument, TActionArgument>(
            TArgument argument,
            TActionArgument actionArgument,
            Func<Type, Action<TActionArgument>> getAction)
        {
            var argumentType = typeof (TArgument);
            var action = getAction(argumentType);
            action(actionArgument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <typeparam name="TArgumentAs"></typeparam>
        /// <param name="argument"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<bool> IfArgumentAsNotNull<TArgument, TArgumentAs>(TArgument argument, Action<TArgumentAs> action) where TArgumentAs : class
        {
            return () => ExecuteIfArgumentAsNotNull(argument, action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<bool> IfArgumentTypeEquals<TArgument>(Type type, Action action)
        {
            return () => ExecuteIfArgumentTypeEquals<TArgument>(type, action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSourceType"></typeparam>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void IfArgumentIs<TSourceType, TTargetType>(TSourceType source, Action<TTargetType> action) where TTargetType : class
        {
            if (source is TTargetType)
            {
                var target = source as TTargetType;
                action(target);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Func<bool> Else(Action action)
        {
            return () => ExecuteElse(action);
        }

        private static bool ExecuteIfArgumentAsNotNull<TArgument, TArgumentAs>(TArgument argument, Action<TArgumentAs> action) where TArgumentAs : class
        {
            var argumentAs = argument as TArgumentAs;
            var selected = argumentAs != null;

            if (selected)
            {
                action(argumentAs);
            }

            return selected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool ExecuteIfArgumentTypeEquals<TArgument>(Type type, Action action)
        {
            var selected = typeof (TArgument) == type;

            if (selected)
            {
                action();
            }

            return selected;
        }

        private static bool ExecuteElse(Action action)
        {
            action();

            const bool selected = true;

            return selected;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    public sealed class MultipleDispatchSelection<TArgument>
    {
        private readonly Func<TArgument, bool>[] selections;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selections"></param>
        public MultipleDispatchSelection(params Func<TArgument, bool>[] selections)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(selections != null);
#endif

            this.selections = selections;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public int Select(TArgument argument)
        {
            var selectedIndex = -1;

            for (var i = 0; i < this.selections.Length; ++i)
            {
                var selection = this.selections[i];
                var selected = selection(argument);
                if (selected)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return selectedIndex;
        }
    }
}