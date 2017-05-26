#if FOUNDATION_3_5

namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AssertMessageParameter
    {
        private readonly string name;
        private readonly object value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public AssertMessageParameter(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

#endif