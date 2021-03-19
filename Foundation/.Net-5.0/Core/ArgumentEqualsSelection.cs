using System;
using Foundation.Assertions;

namespace Foundation.Core
{
    public sealed class ArgumentEqualsSelection<TArgument> where TArgument : IEquatable<TArgument>
    {
        private readonly TArgument _argument;
        private bool _selected;

        public ArgumentEqualsSelection(TArgument argument) => _argument = argument;

        public ArgumentEqualsSelection<TArgument> IfArgumentEquals(TArgument other, Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected)
            {
                _selected = _argument.Equals(other);
                if (_selected)
                    action();
            }

            return this;
        }

        public void Else(Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected)
                action();
        }
    }
}