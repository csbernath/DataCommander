using System;
using Foundation.Assertions;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    public sealed class ArgumentIsSelection<TArgument> where TArgument : class
    {
        private readonly TArgument _argument;
        private bool _selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        public ArgumentIsSelection(TArgument argument)
        {
            _argument = argument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgumentAs"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentIsSelection<TArgument> IfArgumentIs<TArgumentAs>(Action<TArgumentAs> action) where TArgumentAs : class
        {
            Assert.IsNotNull(action);

            if (!_selected)
            {
                var argumentAs = _argument as TArgumentAs;
                _selected = argumentAs != null;
                if (_selected)
                {
                    action(argumentAs);
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentIsSelection<TArgument> IfArgumentIsNull(Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected)
            {
                _selected = _argument == null;
                if (_selected)
                {
                    action();
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentIsSelection<TArgument> If(Func<bool> condition, Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected)
            {
                _selected = condition();
                if (_selected)
                {
                    action();
                }
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public void Else(Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected)
                action();
        }
    }
}