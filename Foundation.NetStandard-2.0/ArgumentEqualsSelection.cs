using System;
using Foundation.Assertions;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    public sealed class ArgumentEqualsSelection<TArgument> where TArgument : IEquatable<TArgument>
    {
        private readonly TArgument _argument;
        private bool _selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        public ArgumentEqualsSelection(TArgument argument)
        {
            _argument = argument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentEqualsSelection<TArgument> IfArgumentEquals(TArgument other, Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected)
            {
                _selected = _argument.Equals(other);
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