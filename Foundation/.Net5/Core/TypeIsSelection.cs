using System;
using Foundation.Assertions;

namespace Foundation.Core
{
    public sealed class TypeIsSelection
    {
        private readonly Type _type;
        private bool _selected;

        public TypeIsSelection(Type type)
        {
            Assert.IsNotNull(type);
            _type = type;
        }

        public TypeIsSelection IfTypeIs<T>(Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected && _type == typeof(T))
            {
                _selected = true;
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