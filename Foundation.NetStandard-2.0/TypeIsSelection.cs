using System;
using Foundation.Diagnostics.Assertions;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TypeIsSelection
    {
        private readonly Type type;
        private bool selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public TypeIsSelection(Type type)
        {
            Assert.IsNotNull(type);
            this.type = type;
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

            if (!selected && type == typeof (T))
            {
                selected = true;
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

            if (!selected)
                action();
        }
    }
}