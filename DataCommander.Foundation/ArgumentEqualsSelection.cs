namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    public sealed class ArgumentEqualsSelection<TArgument> where TArgument : IEquatable<TArgument>
    {
        private readonly TArgument argument;
        private bool selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        public ArgumentEqualsSelection(TArgument argument)
        {
            this.argument = argument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentEqualsSelection<TArgument> IfArgumentEquals(TArgument other, Action action)
        {
            Contract.Requires<ArgumentNullException>(action != null);

            if (!this.selected)
            {
                this.selected = this.argument.Equals(other);
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
            Contract.Requires<ArgumentNullException>(action != null);

            if (!this.selected)
            {
                action();
            }
        }
    }
}