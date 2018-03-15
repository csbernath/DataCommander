using System;
using Foundation.Diagnostics.Contracts;

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
            FoundationContract.Requires<ArgumentNullException>(type != null);
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
            FoundationContract.Requires<ArgumentNullException>(action != null);

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
            FoundationContract.Requires<ArgumentNullException>(action != null);

            if (!selected)
                action();
        }
    }
}