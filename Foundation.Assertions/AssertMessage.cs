#if FOUNDATION_3_5

namespace Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AssertMessage
    {
        private readonly string name;
        private readonly AssertMessageParameterCollection parameters;

        internal AssertMessage( string name )
        {
            this.name = name;
            this.parameters = new AssertMessageParameterCollection();
        }

        internal AssertMessage( string name, AssertMessageParameterCollection parameters )
        {
            this.name = name;
            this.parameters = parameters;
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
        public AssertMessageParameterCollection Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }
}

#endif