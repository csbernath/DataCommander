using System;
using Foundation.Assertions;

namespace Foundation.Core
{
    [Obsolete]
    public static class Selection
    {
        public static ArgumentIsSelection<TArgument> CreateArgumentIsSelection<TArgument>(TArgument argument) where TArgument : class =>
            new(argument);

        public static int Select(Func<bool>[] selections)
        {
            Assert.IsNotNull(selections);

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

        public static void Select<TArgument, TActionArgument>(TArgument argument, TActionArgument actionArgument, Func<Type, Action<TActionArgument>> getAction)
        {
            var argumentType = typeof(TArgument);
            var action = getAction(argumentType);
            action(actionArgument);
        }

        public static Func<bool> IfArgumentAsNotNull<TArgument, TArgumentAs>(TArgument argument, Action<TArgumentAs> action) where TArgumentAs : class =>
            () => ExecuteIfArgumentAsNotNull(argument, action);

        public static Func<bool> IfArgumentTypeEquals<TArgument>(Type type, Action action) => () => ExecuteIfArgumentTypeEquals<TArgument>(type, action);

        public static void IfArgumentIs<TSourceType, TTargetType>(TSourceType source, Action<TTargetType> action) where TTargetType : class
        {
            if (source is TTargetType)
            {
                var target = source as TTargetType;
                action(target);
            }
        }

        public static Func<bool> Else(Action action) => () => ExecuteElse(action);

        private static bool ExecuteIfArgumentAsNotNull<TArgument, TArgumentAs>(TArgument argument, Action<TArgumentAs> action) where TArgumentAs : class
        {
            var argumentAs = argument as TArgumentAs;
            var selected = argumentAs != null;

            if (selected)
                action(argumentAs);

            return selected;
        }

        public static bool ExecuteIfArgumentTypeEquals<TArgument>(Type type, Action action)
        {
            var selected = typeof(TArgument) == type;

            if (selected)
                action();

            return selected;
        }

        private static bool ExecuteElse(Action action)
        {
            action();

            const bool selected = true;

            return selected;
        }
    }

    public sealed class MultipleDispatchSelection<TArgument>
    {
        private readonly Func<TArgument, bool>[] _selections;

        public MultipleDispatchSelection(params Func<TArgument, bool>[] selections)
        {
            Assert.IsNotNull(selections);
            _selections = selections;
        }

        public int Select(TArgument argument)
        {
            var selectedIndex = -1;

            for (var i = 0; i < _selections.Length; ++i)
            {
                var selection = _selections[i];
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