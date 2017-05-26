using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;

namespace Foundation.Configuration
{
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
        public static NameValueCollectionReader Instance => instance.Value;

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollectionReader CurrentType
        {
            get
            {
                var nameValueCollection = ConfigurationManager.AppSettings;

                var stackTrace = new StackTrace(1);
                var stackFrame = stackTrace.GetFrame(0);
                var methodBase = stackFrame.GetMethod();
                var typeName = methodBase.DeclaringType.FullName;
                var prefix = typeName + Type.Delimiter;

                var reader = new PrefixedReader(nameValueCollection, prefix);
                return new NameValueCollectionReader(reader.TryGetValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Reader
        {
            private readonly NameValueCollection _nameValueCollection;

            public Reader(NameValueCollection nameValueCollection)
            {
#if CONTRACTS_FULL
                Contract.Requires<ArgumentNullException>(nameValueCollection != null);
#endif

                this._nameValueCollection = nameValueCollection;
            }

            public bool TryGetValue(string name, out string value)
            {
                value = this._nameValueCollection[name];
                var contains = value != null;
                return contains;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class PrefixedReader
        {
            private readonly NameValueCollection _nameValueCollection;
            private readonly string _prefix;

            public PrefixedReader(NameValueCollection nameValueCollection, string prefix)
            {
#if CONTRACTS_FULL
                Contract.Requires<ArgumentNullException>(nameValueCollection != null);
                Contract.Requires<ArgumentNullException>(prefix != null);
#endif

                this._nameValueCollection = nameValueCollection;
                this._prefix = prefix;
            }

            public bool TryGetValue(string name, out string value)
            {
                string prefixedName;

                if (this._prefix != null)
                {
                    prefixedName = this._prefix + name;
                }
                else
                {
                    prefixedName = name;
                }

                value = this._nameValueCollection[prefixedName];
                var contains = value != null;
                return contains;
            }
        }
    }
}