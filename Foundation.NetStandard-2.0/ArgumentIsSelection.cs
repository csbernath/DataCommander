using System;
using Foundation.Diagnostics.Contracts;

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
            FoundationContract.Requires<ArgumentNullException>(action != null);

            if (!selected)
            {
                var argumentAs = argument as TArgumentAs;
                selected = argumentAs != null;
                if (selected)
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
            FoundationContract.Requires<ArgumentNullException>(action != null);

            if (!selected)
            {
                selected = argument == null;
                if (selected)
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
            FoundationContract.Requires<ArgumentNullException>(action != null);

            if (!selected)
            {
                selected = condition();
                if (selected)
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
            FoundationContract.Requires<ArgumentNullException>(action != null);

            if (!selected)
                action();
        }
    }
}