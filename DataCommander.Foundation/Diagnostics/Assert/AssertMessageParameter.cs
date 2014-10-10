#if FOUNDATION_3_5

namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AssertMessageParameter
    {
        private readonly String name;
        private readonly Object value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public AssertMessageParameter(String name, Object value)
        {
            this.name = name;
            this.value = value;
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
        public Object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

#endif