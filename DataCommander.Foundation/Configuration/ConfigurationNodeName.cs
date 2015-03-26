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
        private static string FromTypeDelimitedName(string name)
        {
            Contract.Requires<ArgumentNullException>(name != null);

            string nodeName = name.Replace(Type.Delimiter, ConfigurationNode.Delimiter);
            return nodeName;
        }

        private static MethodBase GetMethod(StackTrace trace, int frameIndex)
        {
            Contract.Requires<ArgumentNullException>(trace != null);

            StackFrame frame = trace.GetFrame(frameIndex);
            MethodBase method = frame.GetMethod();
            return method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="frameIndex"></param>
        /// <returns></returns>
        internal static string FromNamespace(StackTrace trace, int frameIndex)
        {
            Contract.Requires<ArgumentNullException>(trace != null);

            MethodBase method = GetMethod(trace, frameIndex);
            string name = method.DeclaringType.Namespace;
            string nodeName = FromTypeDelimitedName(name);
            return nodeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FromType(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null);

            string name = type.FullName;
            string nodeName = FromTypeDelimitedName(name);
            return nodeName;
        }

        internal static string FromType(StackTrace trace, int frameIndex)
        {
            Contract.Requires<ArgumentNullException>(trace != null);

            MethodBase method = GetMethod(trace, frameIndex);
            Type type = method.DeclaringType;
            string nodeName = FromType(type);
            return nodeName;
        }

        internal static string FromMethod(MethodBase method)
        {
            Contract.Requires<ArgumentNullException>(method != null);

            string name = method.DeclaringType.FullName + Type.Delimiter + method.Name;
            string nodeName = FromTypeDelimitedName(name);
            return nodeName;
        }

        internal static string FromMethod(StackTrace trace, int frameIndex)
        {
            Contract.Requires<ArgumentNullException>(trace != null);

            MethodBase method = GetMethod(trace, frameIndex);
            string nodeName = FromMethod(method);
            return nodeName;
        }
    }
}