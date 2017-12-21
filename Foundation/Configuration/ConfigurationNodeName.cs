using System;
using System.Diagnostics;
using System.Reflection;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationNodeName
    {
        private static string FromTypeDelimitedName(string name)
        {
            FoundationContract.Requires<ArgumentNullException>(name != null);

            var nodeName = name.Replace(Type.Delimiter, ConfigurationNode.Delimiter);
            return nodeName;
        }

        private static MethodBase GetMethod(StackTrace trace, int frameIndex)
        {
            FoundationContract.Requires<ArgumentNullException>(trace != null);

            var frame = trace.GetFrame(frameIndex);
            var method = frame.GetMethod();
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
            FoundationContract.Requires<ArgumentNullException>(trace != null);

            var method = GetMethod(trace, frameIndex);
            var name = method.DeclaringType.Namespace;
            var nodeName = FromTypeDelimitedName(name);
            return nodeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FromType(Type type)
        {
            FoundationContract.Requires<ArgumentNullException>(type != null);

            var name = type.FullName;
            var nodeName = FromTypeDelimitedName(name);
            return nodeName;
        }

        internal static string FromType(StackTrace trace, int frameIndex)
        {
            FoundationContract.Requires<ArgumentNullException>(trace != null);

            var method = GetMethod(trace, frameIndex);
            var type = method.DeclaringType;
            var nodeName = FromType(type);
            return nodeName;
        }

        internal static string FromMethod(MethodBase method)
        {
            FoundationContract.Requires<ArgumentNullException>(method != null);

            var name = method.DeclaringType.FullName + Type.Delimiter + method.Name;
            var nodeName = FromTypeDelimitedName(name);
            return nodeName;
        }

        internal static string FromMethod(StackTrace trace, int frameIndex)
        {
            FoundationContract.Requires<ArgumentNullException>(trace != null);

            var method = GetMethod(trace, frameIndex);
            var nodeName = FromMethod(method);
            return nodeName;
        }
    }
}