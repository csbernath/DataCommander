namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public static class AppSettings
    {
        private static readonly Lazy<NameValueCollectionReader> instance = new Lazy<NameValueCollectionReader>(CreateInstance);

        private static NameValueCollectionReader CreateInstance()
        {
            var reader = new Reader(ConfigurationManager.AppSettings);
            return new NameValueCollectionReader(reader.TryGetValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollectionReader Instance
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollectionReader CurrentType
        {
            get
            {
                NameValueCollection nameValueCollection = ConfigurationManager.AppSettings;

                var stackTrace = new StackTrace(1);
                StackFrame stackFrame = stackTrace.GetFrame(0);
                MethodBase methodBase = stackFrame.GetMethod();
                string typeName = methodBase.DeclaringType.FullName;
                string prefix = typeName + Type.Delimiter;

                var reader = new PrefixedReader(nameValueCollection, prefix);
                return new NameValueCollectionReader(reader.TryGetValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Reader
        {
            private readonly NameValueCollection nameValueCollection;

            public Reader(NameValueCollection nameValueCollection)
            {
                Contract.Requires<ArgumentNullException>(nameValueCollection != null);

                this.nameValueCollection = nameValueCollection;
            }

            public bool TryGetValue(string name, out string value)
            {
                value = this.nameValueCollection[name];
                bool contains = value != null;
                return contains;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class PrefixedReader
        {
            private readonly NameValueCollection nameValueCollection;
            private readonly string prefix;

            public PrefixedReader(NameValueCollection nameValueCollection, string prefix)
            {
                Contract.Requires<ArgumentNullException>(nameValueCollection != null);
                Contract.Requires<ArgumentNullException>(prefix != null);

                this.nameValueCollection = nameValueCollection;
                this.prefix = prefix;
            }

            public bool TryGetValue(string name, out string value)
            {
                string prefixedName;

                if (this.prefix != null)
                {
                    prefixedName = this.prefix + name;
                }
                else
                {
                    prefixedName = name;
                }

                value = this.nameValueCollection[prefixedName];
                bool contains = value != null;
                return contains;
            }
        }
    }
}