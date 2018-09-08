using System;
using Foundation.Assertions;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TypeIsSelection
    {
        private readonly Type _type;
        private bool _selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TypeIsSelection(Type type)
        {
            Assert.IsNotNull(type);
            _type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public TypeIsSelection IfTypeIs<T>(Action action)
        {
            Assert.IsNotNull(action);

            if (!_selected && _type == typeof (T))
            {
                _selected = true;
                action();
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