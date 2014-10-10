namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationNodeName
    {
        private static String FromTypeDelimitedName( String name )
        {
            Contract.Requires( name != null );

            String nodeName = name.Replace( Type.Delimiter, ConfigurationNode.Delimiter );
            return nodeName;
        }

        private static MethodBase GetMethod( StackTrace trace, Int32 frameIndex )
        {
            Contract.Requires( trace != null );

            StackFrame frame = trace.GetFrame( frameIndex );
            MethodBase method = frame.GetMethod();
            return method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        internal static String FromNamespace( StackTrace trace, Int32 frameIndex )
        {
            Contract.Requires( trace != null );

            MethodBase method = GetMethod( trace, frameIndex );
            String name = method.DeclaringType.Namespace;
            String nodeName = FromTypeDelimitedName( name );
            return nodeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String FromType( Type type )
        {
            Contract.Requires( type != null );

            String name = type.FullName;
            String nodeName = FromTypeDelimitedName( name );
            return nodeName;
        }

        internal static String FromType( StackTrace trace, Int32 frameIndex )
        {
            Contract.Requires( trace != null );

            MethodBase method = GetMethod( trace, frameIndex );
            Type type = method.DeclaringType;
            String nodeName = FromType( type );
            return nodeName;
        }

        internal static String FromMethod( MethodBase method )
        {
            Contract.Requires( method != null );

            String name = method.DeclaringType.FullName + Type.Delimiter + method.Name;
            String nodeName = FromTypeDelimitedName( name );
            return nodeName;
        }

        internal static String FromMethod( StackTrace trace, Int32 frameIndex )
        {
            Contract.Requires( trace != null );

            MethodBase method = GetMethod( trace, frameIndex );
            String nodeName = FromMethod( method );
            return nodeName;
        }
    }
}