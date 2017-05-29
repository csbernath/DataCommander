#if FOUNDATION_3_5
namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// 
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ConditionString = "DEBUG";

        /// <summary>
        /// 
        /// </summary>
        public const string ConditionString2 = "FOUNDATION_ASSERT";

        private static EventHandler<AssertFailedEventArgs> assertFailed;

        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler<AssertFailedEventArgs> AssertFailed
        {
            add
            {
                assertFailed += value;
            }

            remove
            {
                assertFailed -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="expectedName"></param>
        /// <param name="actualName"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void AreEqual<T>( T expected, T actual, string expectedName, string actualName )
        {
            if (!Equals( expected, actual ))
            {
                var message = new AssertMessage( "Assert.AreEqual" );
                AssertMessageParameterCollection parameters = message.Parameters;
                parameters.Add( "expectedName", expectedName );
                parameters.Add( "expected", expected );
                parameters.Add( "actualName", actualName );
                parameters.Add( "actual", actual );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="expectedName"></param>
        /// <param name="actualName"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void AreEqual<T>( IEquatable<T> expected, T actual, string expectedName, string actualName )
        {
            bool equals = expected.Equals( actual );
            if (!equals)
            {
                var message = new AssertMessage( "Assert.AreEqual" );
                var parameters = message.Parameters;
                parameters.Add( "expectedName", expectedName );
                parameters.Add( "expected", expected );
                parameters.Add( "actualName", actualName );
                parameters.Add( "actual", actual );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="comparer"></param>
        /// <param name="arg2"></param>
        /// <param name="arg1Name"></param>
        /// <param name="arg2Name"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Compare<T>(
            IComparable<T> arg1,
            Func<IComparable<T>, T, bool> comparer,
            T arg2,
            string arg1Name,
            string arg2Name )
        {
            IsNotNull( comparer, "comparer" );

            if (!comparer( arg1, arg2 ))
            {
                var message = new AssertMessage( "Assert.Compare" );
                var parameters = message.Parameters;
                parameters.Add( "arg1", arg1 );
                parameters.Add( "arg2", arg2 );
                parameters.Add( "Comparer", comparer );
                parameters.Add( "arg1Name", arg1Name );
                parameters.Add( "arg2Name", arg2Name );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Fail( string name, AssertMessageParameterCollection parameters )
        {
            var message = new AssertMessage( name, parameters );
            Raise( message );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void HasValue<T>( T? value, string name ) where T : struct
        {
            if (!value.HasValue)
            {
                var message = new AssertMessage( "Assert.HasValue" );
                var parameters = message.Parameters;
                parameters.Add( "name", name );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsEnum( Type type )
        {
            IsNotNull( type, "type" );

            if (!type.IsEnum)
            {
                var message = new AssertMessage( "Assert.IsEnum" );
                var parameters = message.Parameters;
                parameters.Add( "type", type );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expectedType"></param>
        /// <param name="name"></param>
        /// <param name="additionalParameters"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsInstanceOfType( object value, Type expectedType, string name, AssertMessageParameterCollection additionalParameters )
        {
            IsNotNull( expectedType, "expectedType" );
            bool isInstanceOfType = expectedType.IsInstanceOfType( value );

            if (!isInstanceOfType)
            {
                var message = new AssertMessage( "Assert.IsInstanceOfType" );
                var parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                parameters.Add( "expectedType", expectedType );

                if (additionalParameters != null)
                {
                    parameters.Add( additionalParameters );
                }

                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsNull( object value, string name )
        {
            if (value != null)
            {
                var message = new AssertMessage( "Assert.IsNull" );
                var parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expectedType"></param>
        /// <param name="name"></param>
        /// <param name="additionalParameters"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Is( object value, Type expectedType, string name, AssertMessageParameterCollection additionalParameters )
        {
            if (value != null)
            {
                IsNotNull( expectedType, "expectedType" );
                bool isInstanceOfType = expectedType.IsInstanceOfType( value );

                if (!isInstanceOfType)
                {
                    var message = new AssertMessage( "Assert.IsInstanceOfType" );
                    var parameters = message.Parameters;
                    parameters.Add( "name", name );
                    parameters.Add( "value", value );
                    parameters.Add( "expectedType", expectedType );

                    if (additionalParameters != null)
                    {
                        parameters.Add( additionalParameters );
                    }

                    Raise( message );
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsNotNull( object value, string name )
        {
            if (value == null)
            {
                var message = new AssertMessage( "Assert.IsNotNull" );
                var parameters = message.Parameters;
                parameters.Add( "name", name );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsNotNull<T>( T? value, string name ) where T : struct
        {
            if (value == null)
            {
                var message = new AssertMessage( "Assert.IsNotNull" );
                var parameters = message.Parameters;
                parameters.Add( "name", name );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsTrue( bool condition, string parameterName, object parameterValue )
        {
            if (!condition)
            {
                var message = new AssertMessage( "Assert.IsTrue" );
                var parameters = message.Parameters;
                parameters.Add( parameterName, parameterValue );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="minValueInclusive"></param>
        /// <param name="maxValueInclusive"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void IsInRange<T>(
            string name,
            IComparable<T> value,
            T minValueInclusive,
            T maxValueInclusive )
        {
            IsNotNull( value, "name" );

            if (value.CompareTo( minValueInclusive ) < 0 || value.CompareTo( maxValueInclusive ) > 0)
            {
                var message = new AssertMessage( "Assert.IsInRange" );
                var parameters = message.Parameters;
                parameters.Add( "name", name );
                parameters.Add( "value", value );
                parameters.Add( "minValueInclusive", minValueInclusive );
                parameters.Add( "maxValueInclusive", maxValueInclusive );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="argName1"></param>
        /// <param name="argName2"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void ReferenceEquals( object arg1, object arg2, string argName1, string argName2 )
        {
            if (!ReferenceEquals( arg1, arg2 ))
            {
                var message = new AssertMessage( "Assert.AreEqual" );
                var parameters = message.Parameters;
                parameters.Add( "arg1", arg1 );
                parameters.Add( "arg2", arg2 );
                parameters.Add( "argName1", argName1 );
                parameters.Add( "argName2", argName2 );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="conditionString"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Requires( bool condition, string conditionString )
        {
            if (!condition)
            {
                var message = new AssertMessage( "Assert.Requires" );
                var parameters = message.Parameters;
                parameters.Add( "conditionString", conditionString );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="conditionString"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Requires( Func<bool> condition, string conditionString )
        {
            IsNotNull( condition, "condition" );

            if (!condition())
            {
                var message = new AssertMessage( "Assert.Requires" );
                var parameters = message.Parameters;
                parameters.Add( "conditionString", conditionString );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="parameters"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Requires( bool condition, AssertMessageParameterCollection parameters )
        {
            if (!condition)
            {
                var message = new AssertMessage( "Assert.Requires", parameters );
                Raise( message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="conditionString"></param>
        /// <param name="argName"></param>
        /// <param name="arg"></param>
        [Conditional( ConditionString )]
        [Conditional( ConditionString2 )]
        [DebuggerStepThrough]
        [Obsolete]
        public static void Requires<T>(
            Func<T, bool> condition,
            string conditionString,
            string argName,
            T arg )
        {
            IsNotNull( condition, "condition" );

            if (!condition( arg ))
            {
                var message = new AssertMessage( "Assert.Requires" );
                var parameters = message.Parameters;
                parameters.Add( "condition", condition );
                parameters.Add( "conditionString", conditionString );
                parameters.Add( "argName", argName );
                parameters.Add( "arg", arg );
                Raise( message );
            }
        }

        [Obsolete]
        internal static void Raise( AssertMessage message )
        {
            if (assertFailed != null)
            {
                var e = new AssertFailedEventArgs( message );
                assertFailed( null, e );
            }
            else
            {
                throw new AssertFailedException( message );
            }
        }
    }
}
#endif