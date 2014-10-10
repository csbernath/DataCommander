namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class CommandLineArgument
    {
        private readonly Int32 index;
        private readonly String name;
        private readonly String value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public CommandLineArgument(
            Int32 index,
            String name,
            String value)
        {
            this.index = index;
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Value
        {
            get
            {
                return this.value;
            }
        }
    }
}