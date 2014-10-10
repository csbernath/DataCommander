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
        private static Lazy<NameValueCollectionReader> instance = new Lazy<NameValueCollectionReader>( CreateInstance );

        private static NameValueCollectionReader CreateInstance()
        {
            var reader = new Reader( ConfigurationManager.AppSettings );
            return new NameValueCollectionReader( reader.TryGetValue );
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

                var stackTrace = new StackTrace( 1 );
                StackFrame stackFrame = stackTrace.GetFrame( 0 );
                MethodBase methodBase = stackFrame.GetMethod();
                String typeName = methodBase.DeclaringType.FullName;
                String prefix = typeName + Type.Delimiter;

                var reader = new PrefixedReader( nameValueCollection, prefix );
                return new NameValueCollectionReader( reader.TryGetValue );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class Reader
        {
            private NameValueCollection nameValueCollection;

            public Reader( NameValueCollection nameValueCollection )
            {
                Contract.Requires( nameValueCollection != null );

                this.nameValueCollection = nameValueCollection;
            }

            public Boolean TryGetValue( String name, out String value )
            {
                value = this.nameValueCollection[ name ];
                Boolean contains = value != null;
                return contains;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class PrefixedReader
        {
            private NameValueCollection nameValueCollection;
            private String prefix;

            public PrefixedReader( NameValueCollection nameValueCollection, String prefix )
            {
                Contract.Requires( nameValueCollection != null );
                Contract.Requires( prefix != null );

                this.nameValueCollection = nameValueCollection;
                this.prefix = prefix;
            }

            public Boolean TryGetValue( String name, out String value )
            {
                String prefixedName;

                if (this.prefix != null)
                {
                    prefixedName = this.prefix + name;
                }
                else
                {
                    prefixedName = name;
                }

                value = this.nameValueCollection[ prefixedName ];
                Boolean contains = value != null;
                return contains;
            }
        }
    }
}