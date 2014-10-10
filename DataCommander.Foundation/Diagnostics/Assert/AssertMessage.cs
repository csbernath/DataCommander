#if FOUNDATION_3_5

namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AssertMessage
    {
        private readonly String name;
        private readonly AssertMessageParameterCollection parameters;

        internal AssertMessage( String name )
        {
            this.name = name;
            this.parameters = new AssertMessageParameterCollection();
        }

        internal AssertMessage( String name, AssertMessageParameterCollection parameters )
        {
            this.name = name;
            this.parameters = parameters;
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