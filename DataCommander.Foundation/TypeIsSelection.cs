using System;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(type != null);
#endif
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(action != null);
#endif

            if (!this.selected && this.type == typeof (T))
            {
                this.selected = true;
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(action != null);
#endif

            if (!this.selected)
            {
                action();
            }
        }
    }
}