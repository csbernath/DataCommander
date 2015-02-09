namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class Selection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static ArgumentEqualsSelection<TArgument> CreateArgumentEqualsSelection<TArgument>(TArgument argument) where TArgument : IEquatable<TArgument>
        {
            return new ArgumentEqualsSelection<TArgument>(argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static ArgumentIsSelection<TArgument> CreateArgumentIsSelection<TArgument>(TArgument argument) where TArgument : class
        {
            return new ArgumentIsSelection<TArgument>(argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeIsSelection CreateTypeIsSelection(Type type)
        {
            return new TypeIsSelection(type);
        }
    }

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
            Contract.Requires(action != null);

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
            Contract.Requires(action != null);

            if (!this.selected)
            {
                action();
            }
        }
    }

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
            Contract.Requires(action != null);

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
        /// <param name="condition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ArgumentIsSelection<TArgument> If(Func<bool> condition, Action action)
        {
            Contract.Requires(action != null);

            if (!this.selected && condition())
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
            Contract.Requires(action != null);

            if (!this.selected)
            {
                action();
            }
        }
    }

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
            Contract.Requires(action != null);

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
            Contract.Requires(action != null);

            if (!this.selected)
            {
                action();
            }
        }
    }
}