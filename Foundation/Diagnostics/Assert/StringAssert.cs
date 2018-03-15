#if FOUNDATION_3_5

namespace Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public static class StringAssert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void IsNullOrEmpty( string value, string name )
        {
            if (!string.IsNullOrEmpty( value ))
            {
                var message = new AssertMessage( "StringAssert.IsNullOrEmpty" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void IsNullOrWhiteSpace( string value, string name )
        {
            if (!value.IsNullOrWhiteSpace())
            {
                var message = new AssertMessage( "StringAssert.IsNullOrWhiteSpace" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void IsNotNullAndNotEmpty(
            string value,
            string name )
        {
            if (string.IsNullOrEmpty( value ))
            {
                AssertMessage message = new AssertMessage( "StringAssert.IsNotNullAndNotEmpty" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                Assert.Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( Assert.ConditionString )]
        [Conditional( Assert.ConditionString2 )]
        [DebuggerStepThrough]
        public static void IsNotNullAndNotWhiteSpace( string value, string name )
        {
            if (value.IsNullOrWhiteSpace())
            {
                AssertMessage message = new AssertMessage( "StringAssert.IsNotNullAndNotWhiteSpace" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                Assert.Raise( message );
            }
        }
    }
}

#endif