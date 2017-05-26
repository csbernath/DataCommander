using System;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    public sealed class ArgumentIsSelection<TArgument> where TArgument : class
    {
        private readonly TArgument argument;
        private bool selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        public ArgumentIsSelection(TArgument argument)
        {
            this.argument = argument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgumentAs"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentIsSelection<TArgument> IfArgumentIs<TArgumentAs>(Action<TArgumentAs> action) where TArgumentAs : class
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(action != null);
#endif

            if (!this.selected)
            {
                var argumentAs = this.argument as TArgumentAs;
                this.selected = argumentAs != null;
                if (this.selected)
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(action != null);
#endif

            if (!this.selected)
            {
                this.selected = this.argument == null;
                if (this.selected)
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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(action != null);
#endif

            if (!this.selected)
            {
                this.selected = condition();
                if (this.selected)
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